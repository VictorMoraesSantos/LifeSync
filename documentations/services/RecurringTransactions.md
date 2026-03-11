# Feature: Transações Recorrentes

## Visão Geral

A feature de **Transações Recorrentes** permite que os usuários, ao criar uma transação, marquem se ela é recorrente ou não. Caso seja recorrente, campos adicionais de frequência são exibidos para configurar a repetição (diário, semanal, mensal, anual).

**Exemplos de uso:**
- Aluguel mensal
- Conta de luz/água/internet mensal
- Salário mensal
- Assinaturas semanais ou mensais
- Despesas diárias fixas (transporte, alimentação)

---

## Decisão Arquitetural: Por que uma entidade separada?

### O fluxo do usuário

```
1. Usuário cria Transaction com IsRecurring = true
2. Se IsRecurring = true → exibe campos de recorrência (frequência, data fim, etc.)
3. Sistema salva a Transaction (registro financeiro real)
4. Sistema cria um RecurrenceSchedule vinculado àquela Transaction (agendamento)
5. Background Service lê os schedules pendentes e gera cópias da Transaction original
```

### Por que NÃO colocar tudo na Transaction?

| Problema | Explicação |
|----------|-----------|
| **Mistura de conceitos** | `Transaction` = fato financeiro que aconteceu. Campos como `NextOccurrence`, `MaxOccurrences`, `Frequency` não fazem sentido em transações normais |
| **Poluição na listagem** | Ao listar transações, os campos de recorrência estariam null em 90%+ dos registros |
| **Filtros confusos** | Campos nullable dificultam queries e especificações |
| **Princípio SRP** | Uma entidade não deve ser ao mesmo tempo "registro" e "agendador" |

### A solução: `RecurrenceSchedule` (entidade enxuta)

A entidade `RecurrenceSchedule` **não duplica os dados da Transaction**. Ela apenas armazena:
- **Referência** à Transaction original (`SourceTransactionId`)
- **Configuração de repetição** (frequência, datas, limites)
- **Estado do agendamento** (próxima execução, contador, ativo/inativo)

O Background Service lê o schedule, busca a Transaction original e **copia seus dados** para criar novas transações.

```
┌──────────────────┐         ┌────────────────────────┐
│   Transaction    │ 1───1   │  RecurrenceSchedule    │
│──────────────────│         │────────────────────────│
│ Id               │◄────────│ SourceTransactionId    │
│ UserId           │         │ Frequency              │
│ CategoryId       │         │ StartDate              │
│ PaymentMethod    │         │ EndDate                │
│ TransactionType  │         │ NextOccurrence         │
│ Amount           │         │ MaxOccurrences         │
│ Description      │         │ OccurrencesGenerated   │
│ TransactionDate  │         │ IsActive               │
│ IsRecurring ────►│         └────────────────────────┘
└──────────────────┘
        │
        │ (Background Service copia dados)
        ▼
┌──────────────────┐
│ Transaction (N)  │  ← transações geradas automaticamente
│ IsRecurring=true │
└──────────────────┘
```

---

## Arquitetura Atual (Estado Atual)

O sistema já possui a base:

- `Transaction.IsRecurring` (bool) — flag que indica se a transação é recorrente
- `TransactionErrors.RecurringTransactionError` — erro de domínio já definido
- Toda a cadeia (Entity → DTO → Command → Handler → Service → Controller) já propaga `IsRecurring`

**O que falta implementar:**
- Enum `RecurrenceFrequency`
- Entidade `RecurrenceSchedule` (vinculada à Transaction)
- Erros de domínio específicos
- CQRS para gerenciar schedules
- Repository + Service
- Background Service para processar recorrências
- Ajustar o fluxo de criação de Transaction para criar o schedule junto
- Migration para a nova tabela

---

## 1. Domain Layer

### 1.1 Enum — `RecurrenceFrequency`

**Arquivo:** `Financial.Domain/Enums/RecurrenceFrequency.cs`

```csharp
namespace Financial.Domain.Enums
{
    public enum RecurrenceFrequency
    {
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Yearly = 4
    }

    public static class RecurrenceFrequencyExtensions
    {
        public static string ToFriendlyString(this RecurrenceFrequency frequency)
        {
            return frequency switch
            {
                RecurrenceFrequency.Daily => "Diário",
                RecurrenceFrequency.Weekly => "Semanal",
                RecurrenceFrequency.Monthly => "Mensal",
                RecurrenceFrequency.Yearly => "Anual",
                _ => "Desconhecido"
            };
        }
    }
}
```

### 1.2 Entidade — `RecurrenceSchedule`

**Arquivo:** `Financial.Domain/Entities/RecurrenceSchedule.cs`

> Entidade enxuta — apenas configuração de agendamento, sem duplicar dados da Transaction.

```csharp
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Financial.Domain.Enums;
using Financial.Domain.Errors;

namespace Financial.Domain.Entities
{
    public class RecurrenceSchedule : BaseEntity<int>
    {
        public int SourceTransactionId { get; private set; }
        public Transaction SourceTransaction { get; private set; }
        public RecurrenceFrequency Frequency { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public DateTime NextOccurrence { get; private set; }
        public int? MaxOccurrences { get; private set; }
        public int OccurrencesGenerated { get; private set; } = 0;
        public bool IsActive { get; private set; } = true;

        private RecurrenceSchedule() { }

        public RecurrenceSchedule(
            int sourceTransactionId,
            RecurrenceFrequency frequency,
            DateTime startDate,
            DateTime? endDate = null,
            int? maxOccurrences = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sourceTransactionId);

            if (endDate.HasValue && endDate.Value <= startDate)
                throw new DomainException(RecurrenceScheduleErrors.EndDateBeforeStartDate);

            if (maxOccurrences.HasValue && maxOccurrences.Value <= 0)
                throw new DomainException(RecurrenceScheduleErrors.InvalidMaxOccurrences);

            SourceTransactionId = sourceTransactionId;
            Frequency = frequency;
            StartDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            EndDate = endDate.HasValue ? DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc) : null;
            NextOccurrence = CalculateNextOccurrence(
                DateTime.SpecifyKind(startDate, DateTimeKind.Utc), frequency);
            MaxOccurrences = maxOccurrences;
        }

        public void Update(
            RecurrenceFrequency frequency,
            DateTime? endDate = null,
            int? maxOccurrences = null)
        {
            if (endDate.HasValue && endDate.Value <= StartDate)
                throw new DomainException(RecurrenceScheduleErrors.EndDateBeforeStartDate);

            if (maxOccurrences.HasValue && maxOccurrences.Value <= 0)
                throw new DomainException(RecurrenceScheduleErrors.InvalidMaxOccurrences);

            Frequency = frequency;
            EndDate = endDate.HasValue ? DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc) : null;
            MaxOccurrences = maxOccurrences;
            MarkAsUpdated();
        }

        /// <summary>
        /// Gera uma nova Transaction copiando os dados da Transaction original
        /// e avança NextOccurrence.
        /// </summary>
        public Transaction GenerateTransaction()
        {
            if (!IsActive)
                throw new DomainException(RecurrenceScheduleErrors.InactiveSchedule);

            if (!CanGenerateNext())
                throw new DomainException(RecurrenceScheduleErrors.MaxOccurrencesReached);

            if (SourceTransaction == null)
                throw new DomainException(RecurrenceScheduleErrors.SourceTransactionNotLoaded);

            // Copia os dados da transação original
            var transaction = new Transaction(
                SourceTransaction.UserId,
                SourceTransaction.CategoryId,
                SourceTransaction.PaymentMethod,
                SourceTransaction.TransactionType,
                SourceTransaction.Amount,
                SourceTransaction.Description,
                NextOccurrence,
                isRecurring: true);

            OccurrencesGenerated++;
            NextOccurrence = CalculateNextOccurrence(NextOccurrence, Frequency);

            if (MaxOccurrences.HasValue && OccurrencesGenerated >= MaxOccurrences.Value)
                IsActive = false;

            if (EndDate.HasValue && NextOccurrence > EndDate.Value)
                IsActive = false;

            MarkAsUpdated();
            return transaction;
        }

        public bool CanGenerateNext()
        {
            if (!IsActive) return false;
            if (MaxOccurrences.HasValue && OccurrencesGenerated >= MaxOccurrences.Value) return false;
            if (EndDate.HasValue && NextOccurrence > EndDate.Value) return false;
            return true;
        }

        public void Deactivate()
        {
            IsActive = false;
            MarkAsUpdated();
        }

        public void Activate()
        {
            IsActive = true;
            MarkAsUpdated();
        }

        private static DateTime CalculateNextOccurrence(DateTime current, RecurrenceFrequency frequency)
        {
            return frequency switch
            {
                RecurrenceFrequency.Daily => current.AddDays(1),
                RecurrenceFrequency.Weekly => current.AddDays(7),
                RecurrenceFrequency.Monthly => current.AddMonths(1),
                RecurrenceFrequency.Yearly => current.AddYears(1),
                _ => throw new ArgumentOutOfRangeException(nameof(frequency))
            };
        }
    }
}
```

### 1.3 Erros de Domínio — `RecurrenceScheduleErrors`

**Arquivo:** `Financial.Domain/Errors/RecurrenceScheduleErrors.cs`

```csharp
using BuildingBlocks.Results;

namespace Financial.Domain.Errors
{
    public static class RecurrenceScheduleErrors
    {
        // Erros de validação
        public static Error InvalidId => Error.Failure("O ID do agendamento deve ser maior que zero");
        public static Error InvalidFrequency => Error.Failure("A frequência de recorrência é inválida");
        public static Error InvalidStartDate => Error.Failure("A data de início é obrigatória");
        public static Error EndDateBeforeStartDate => Error.Failure("A data final deve ser posterior à data de início");
        public static Error InvalidMaxOccurrences => Error.Failure("O número máximo de ocorrências deve ser maior que zero");
        public static Error InvalidSourceTransaction => Error.Failure("A transação de origem é obrigatória");

        // Erros de operação
        public static Error NotFound(int id) => Error.NotFound($"Agendamento com ID {id} não encontrado");
        public static Error NotFound() => Error.NotFound("Agendamentos não encontrados");
        public static Error CreateError => Error.Problem("Erro ao criar agendamento de recorrência");
        public static Error UpdateError => Error.Problem("Erro ao atualizar agendamento de recorrência");
        public static Error DeleteError => Error.Problem("Erro ao excluir agendamento de recorrência");

        // Erros de negócio
        public static Error InactiveSchedule => Error.Failure("Este agendamento está inativo");
        public static Error MaxOccurrencesReached => Error.Failure("Número máximo de ocorrências atingido");
        public static Error SourceTransactionNotLoaded => Error.Failure("A transação de origem não foi carregada");
        public static Error SourceTransactionNotRecurring => Error.Failure("A transação de origem não está marcada como recorrente");
        public static Error ScheduleAlreadyExists => Error.Failure("Já existe um agendamento para esta transação");
        public static Error ProcessingError => Error.Problem("Erro ao processar transações recorrentes");
    }
}
```

### 1.4 Adicionar navegação na Transaction

**Edição em:** `Financial.Domain/Entities/Transaction.cs`

```csharp
// Adicionar propriedade de navegação (relação 1:1 opcional)
public RecurrenceSchedule? RecurrenceSchedule { get; private set; }
```

### 1.5 Filtro de Query — `RecurrenceScheduleQueryFilter`

**Arquivo:** `Financial.Domain/Filters/RecurrenceScheduleQueryFilter.cs`

```csharp
using Core.Domain.Filters;
using Financial.Domain.Enums;

namespace Financial.Domain.Filters
{
    public record RecurrenceScheduleQueryFilter(
        int? Id = null,
        int? SourceTransactionId = null,
        int? UserId = null,
        RecurrenceFrequency? Frequency = null,
        bool? IsActive = null,
        DateTime? StartDateFrom = null,
        DateTime? StartDateTo = null,
        DateTime? CreatedAt = null,
        DateTime? UpdatedAt = null,
        bool? IsDeleted = null,
        string? SortBy = null,
        bool? SortDesc = null,
        int? Page = null,
        int? PageSize = null)
        : DomainQueryFilter(Id, CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);
}
```

### 1.6 Specification — `RecurrenceScheduleSpecification`

**Arquivo:** `Financial.Domain/Filters/Specifications/RecurrenceScheduleSpecification.cs`

```csharp
using Core.Domain.Filters.Specifications;
using Financial.Domain.Entities;

namespace Financial.Domain.Filters.Specifications
{
    public class RecurrenceScheduleSpecification : BaseSpecification<RecurrenceSchedule>
    {
        public RecurrenceScheduleSpecification(RecurrenceScheduleQueryFilter filter)
        {
            AddIf(filter.Id.HasValue,
                r => r.Id == filter.Id!.Value);

            AddIf(filter.SourceTransactionId.HasValue,
                r => r.SourceTransactionId == filter.SourceTransactionId!.Value);

            // Filtra pelo UserId da Transaction de origem
            AddIf(filter.UserId.HasValue,
                r => r.SourceTransaction.UserId == filter.UserId!.Value);

            AddIf(filter.Frequency.HasValue,
                r => r.Frequency == filter.Frequency!.Value);

            AddIf(filter.IsActive.HasValue,
                r => r.IsActive == filter.IsActive!.Value);

            AddIf(filter.StartDateFrom.HasValue,
                r => r.StartDate >= filter.StartDateFrom!.Value);

            AddIf(filter.StartDateTo.HasValue,
                r => r.StartDate <= filter.StartDateTo!.Value);

            AddIf(filter.IsDeleted.HasValue,
                r => r.IsDeleted == filter.IsDeleted!.Value);
        }
    }
}
```

### 1.7 Repository Interface — `IRecurrenceScheduleRepository`

**Arquivo:** `Financial.Domain/Repositories/IRecurrenceScheduleRepository.cs`

```csharp
using Core.Domain.Repositories;
using Financial.Domain.Entities;
using Financial.Domain.Filters;

namespace Financial.Domain.Repositories
{
    public interface IRecurrenceScheduleRepository
        : IRepository<RecurrenceSchedule, int, RecurrenceScheduleQueryFilter>
    {
        Task<RecurrenceSchedule?> GetBySourceTransactionIdAsync(
            int sourceTransactionId, CancellationToken cancellationToken = default);

        Task<IEnumerable<RecurrenceSchedule>> GetActiveByUserIdAsync(
            int userId, CancellationToken cancellationToken = default);

        Task<IEnumerable<RecurrenceSchedule>> GetDueSchedulesAsync(
            DateTime referenceDate, CancellationToken cancellationToken = default);
    }
}
```

---

## 2. Application Layer

### 2.1 DTOs

**Arquivo:** `Financial.Application/DTOs/RecurrenceSchedule/RecurrenceScheduleDTO.cs`

```csharp
using Core.Application.DTO;
using Financial.Application.DTOs.Transaction;
using Financial.Domain.Enums;

namespace Financial.Application.DTOs.RecurrenceSchedule
{
    public record RecurrenceScheduleDTO(
        int Id,
        int SourceTransactionId,
        TransactionDTO SourceTransaction,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        RecurrenceFrequency Frequency,
        DateTime StartDate,
        DateTime? EndDate,
        DateTime NextOccurrence,
        int? MaxOccurrences,
        int OccurrencesGenerated,
        bool IsActive)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
```

**Arquivo:** `Financial.Application/DTOs/RecurrenceSchedule/CreateRecurrenceScheduleDTO.cs`

> Este DTO é usado internamente. O usuário cria via `CreateTransactionCommand` com os campos extras.

```csharp
using Financial.Domain.Enums;

namespace Financial.Application.DTOs.RecurrenceSchedule
{
    public record CreateRecurrenceScheduleDTO(
        int SourceTransactionId,
        RecurrenceFrequency Frequency,
        DateTime StartDate,
        DateTime? EndDate = null,
        int? MaxOccurrences = null);
}
```

**Arquivo:** `Financial.Application/DTOs/RecurrenceSchedule/UpdateRecurrenceScheduleDTO.cs`

```csharp
using Financial.Domain.Enums;

namespace Financial.Application.DTOs.RecurrenceSchedule
{
    public record UpdateRecurrenceScheduleDTO(
        int Id,
        RecurrenceFrequency Frequency,
        DateTime? EndDate = null,
        int? MaxOccurrences = null);
}
```

**Arquivo:** `Financial.Application/DTOs/RecurrenceSchedule/RecurrenceScheduleFilterDTO.cs`

```csharp
using Financial.Domain.Enums;

namespace Financial.Application.DTOs.RecurrenceSchedule
{
    public record RecurrenceScheduleFilterDTO(
        int? Id = null,
        int? SourceTransactionId = null,
        int? UserId = null,
        RecurrenceFrequency? Frequency = null,
        bool? IsActive = null,
        DateTime? StartDateFrom = null,
        DateTime? StartDateTo = null,
        DateTime? CreatedAt = null,
        DateTime? UpdatedAt = null,
        bool? IsDeleted = null,
        string? SortBy = null,
        bool? SortDesc = null,
        int? Page = null,
        int? PageSize = null);
}
```

### 2.2 Mapper — `RecurrenceScheduleMapper`

**Arquivo:** `Financial.Application/Mappings/RecurrenceScheduleMapper.cs`

```csharp
using Financial.Application.DTOs.RecurrenceSchedule;
using Financial.Domain.Entities;

namespace Financial.Application.Mappings
{
    public static class RecurrenceScheduleMapper
    {
        public static RecurrenceSchedule ToEntity(CreateRecurrenceScheduleDTO dto)
        {
            RecurrenceSchedule entity = new(
                dto.SourceTransactionId,
                dto.Frequency,
                dto.StartDate,
                dto.EndDate,
                dto.MaxOccurrences);
            return entity;
        }

        public static RecurrenceScheduleDTO ToDTO(RecurrenceSchedule entity)
        {
            RecurrenceScheduleDTO dto = new(
                entity.Id,
                entity.SourceTransactionId,
                entity.SourceTransaction != null
                    ? TransactionMapper.ToDTO(entity.SourceTransaction)
                    : null,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.Frequency,
                entity.StartDate,
                entity.EndDate,
                entity.NextOccurrence,
                entity.MaxOccurrences,
                entity.OccurrencesGenerated,
                entity.IsActive);
            return dto;
        }
    }
}
```

### 2.3 Alterar o `CreateTransactionCommand` — Aceitar campos de recorrência

**Edição em:** `Financial.Application/Features/Transactions/Commands/Create/CreateTransactionCommand.cs`

```csharp
using BuildingBlocks.CQRS.Requests.Commands;
using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;

namespace Financial.Application.Features.Transactions.Commands.Create
{
    public record CreateTransactionCommand(
        int UserId,
        int? CategoryId,
        PaymentMethod PaymentMethod,
        TransactionType TransactionType,
        Money Amount,
        string Description,
        DateTime TransactionDate,
        bool IsRecurring = false,
        // Campos de recorrência (obrigatórios apenas quando IsRecurring = true)
        RecurrenceFrequency? Frequency = null,
        DateTime? RecurrenceEndDate = null,
        int? MaxOccurrences = null) : ICommand<CreateTransactionResult>;

    public record CreateTransactionResult(int TransactionId);
}
```

### 2.4 Alterar o `CreateTransactionCommandValidator`

**Edição em:** `Financial.Application/Features/Transactions/Commands/Create/CreateTransactionCommandValidator.cs`

```csharp
using FluentValidation;

namespace Financial.Application.Features.Transactions.Commands.Create
{
    public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
    {
        public CreateTransactionCommandValidator()
        {
            RuleFor(command => command.CategoryId)
                .GreaterThan(0).WithMessage("O identificador da categoria deve ser maior que zero.")
                .When(command => command.CategoryId.HasValue);

            RuleFor(command => command.PaymentMethod)
                .IsInEnum().WithMessage("O método de pagamento informado é inválido.");

            RuleFor(command => command.TransactionType)
                .IsInEnum().WithMessage("O tipo de transação informado é inválido.");

            RuleFor(command => command.Amount)
                .NotNull().WithMessage("O valor da transação é obrigatório.")
                .Must(amount => amount.Amount != 0).WithMessage("O valor da transação não pode ser zero.")
                .Must(amount => amount.Amount > 0 || amount.Amount < 0).WithMessage("O valor da transação deve ser positivo para receitas ou negativo para despesas.");

            RuleFor(command => command.Description)
                .NotEmpty().WithMessage("A descrição da transação é obrigatória.")
                .MaximumLength(200).WithMessage("A descrição da transação deve ter no máximo 200 caracteres.");

            RuleFor(command => command.TransactionDate)
                .NotEmpty().WithMessage("A data da transação é obrigatória.")
                .LessThanOrEqualTo(DateTime.Now.AddDays(1)).WithMessage("A data da transação não pode ser futura além de 1 dia.");

            // Validações condicionais para recorrência
            When(command => command.IsRecurring, () =>
            {
                RuleFor(command => command.Frequency)
                    .NotNull().WithMessage("A frequência é obrigatória para transações recorrentes.")
                    .IsInEnum().WithMessage("A frequência de recorrência é inválida.");

                RuleFor(command => command.RecurrenceEndDate)
                    .GreaterThan(command => command.TransactionDate)
                    .WithMessage("A data final da recorrência deve ser posterior à data da transação.")
                    .When(command => command.RecurrenceEndDate.HasValue);

                RuleFor(command => command.MaxOccurrences)
                    .GreaterThan(0)
                    .WithMessage("O número máximo de ocorrências deve ser maior que zero.")
                    .When(command => command.MaxOccurrences.HasValue);
            });
        }
    }
}
```

### 2.5 Alterar o `CreateTransactionCommandHandler` — Criar Schedule junto

**Edição em:** `Financial.Application/Features/Transactions/Commands/Create/CreateTransactionCommandHandler.cs`

```csharp
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.RecurrenceSchedule;
using Financial.Application.DTOs.Transaction;

namespace Financial.Application.Features.Transactions.Commands.Create
{
    public class CreateTransactionCommandHandler
        : ICommandHandler<CreateTransactionCommand, CreateTransactionResult>
    {
        private readonly ITransactionService _transactionService;
        private readonly IRecurrenceScheduleService _recurrenceScheduleService;

        public CreateTransactionCommandHandler(
            ITransactionService transactionService,
            IRecurrenceScheduleService recurrenceScheduleService)
        {
            _transactionService = transactionService;
            _recurrenceScheduleService = recurrenceScheduleService;
        }

        public async Task<Result<CreateTransactionResult>> Handle(
            CreateTransactionCommand command, CancellationToken cancellationToken)
        {
            // 1. Criar a transação
            var dto = new CreateTransactionDTO(
                command.UserId,
                command.CategoryId,
                command.PaymentMethod,
                command.TransactionType,
                command.Amount,
                command.Description,
                command.TransactionDate,
                command.IsRecurring);

            var result = await _transactionService.CreateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<CreateTransactionResult>(result.Error!);

            var transactionId = result.Value!;

            // 2. Se recorrente, criar o schedule vinculado
            if (command.IsRecurring && command.Frequency.HasValue)
            {
                var scheduleDto = new CreateRecurrenceScheduleDTO(
                    transactionId,
                    command.Frequency.Value,
                    command.TransactionDate,
                    command.RecurrenceEndDate,
                    command.MaxOccurrences);

                var scheduleResult = await _recurrenceScheduleService.CreateAsync(
                    scheduleDto, cancellationToken);

                if (!scheduleResult.IsSuccess)
                    return Result.Failure<CreateTransactionResult>(scheduleResult.Error!);
            }

            return Result.Success(new CreateTransactionResult(transactionId));
        }
    }
}
```

### 2.6 Service Contract — `IRecurrenceScheduleService`

**Arquivo:** `Financial.Application/Contracts/IRecurrenceScheduleService.cs`

```csharp
using BuildingBlocks.Results;
using Core.Application.Contracts;
using Financial.Application.DTOs.RecurrenceSchedule;

namespace Financial.Application.Contracts
{
    public interface IRecurrenceScheduleService
        : IReadService<RecurrenceScheduleDTO, int, RecurrenceScheduleFilterDTO>,
          ICreateService<CreateRecurrenceScheduleDTO>,
          IUpdateService<UpdateRecurrenceScheduleDTO>,
          IDeleteService<int>
    {
        Task<Result<RecurrenceScheduleDTO?>> GetBySourceTransactionIdAsync(
            int sourceTransactionId, CancellationToken cancellationToken = default);

        Task<Result<IEnumerable<RecurrenceScheduleDTO>>> GetActiveByUserIdAsync(
            int userId, CancellationToken cancellationToken = default);

        Task<Result<bool>> DeactivateAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<bool>> ActivateAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<int>> ProcessDueSchedulesAsync(CancellationToken cancellationToken = default);
    }
}
```

### 2.7 Commands CQRS — RecurrenceSchedule

#### Update

**Arquivo:** `Financial.Application/Features/RecurrenceSchedules/Commands/Update/UpdateRecurrenceScheduleCommand.cs`

```csharp
using BuildingBlocks.CQRS.Requests.Commands;
using Financial.Domain.Enums;

namespace Financial.Application.Features.RecurrenceSchedules.Commands.Update
{
    public record UpdateRecurrenceScheduleCommand(
        int Id,
        RecurrenceFrequency Frequency,
        DateTime? EndDate = null,
        int? MaxOccurrences = null) : ICommand<UpdateRecurrenceScheduleResult>;

    public record UpdateRecurrenceScheduleResult(bool IsSuccess);
}
```

**Arquivo:** `Financial.Application/Features/RecurrenceSchedules/Commands/Update/UpdateRecurrenceScheduleCommandHandler.cs`

```csharp
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.RecurrenceSchedule;

namespace Financial.Application.Features.RecurrenceSchedules.Commands.Update
{
    public class UpdateRecurrenceScheduleCommandHandler
        : ICommandHandler<UpdateRecurrenceScheduleCommand, UpdateRecurrenceScheduleResult>
    {
        private readonly IRecurrenceScheduleService _service;

        public UpdateRecurrenceScheduleCommandHandler(IRecurrenceScheduleService service)
        {
            _service = service;
        }

        public async Task<Result<UpdateRecurrenceScheduleResult>> Handle(
            UpdateRecurrenceScheduleCommand command, CancellationToken cancellationToken)
        {
            var dto = new UpdateRecurrenceScheduleDTO(
                command.Id,
                command.Frequency,
                command.EndDate,
                command.MaxOccurrences);

            var result = await _service.UpdateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<UpdateRecurrenceScheduleResult>(result.Error!);

            return Result.Success(new UpdateRecurrenceScheduleResult(result.Value!));
        }
    }
}
```

#### Deactivate

**Arquivo:** `Financial.Application/Features/RecurrenceSchedules/Commands/Deactivate/DeactivateRecurrenceScheduleCommand.cs`

```csharp
using BuildingBlocks.CQRS.Requests.Commands;

namespace Financial.Application.Features.RecurrenceSchedules.Commands.Deactivate
{
    public record DeactivateRecurrenceScheduleCommand(int Id)
        : ICommand<DeactivateRecurrenceScheduleResult>;

    public record DeactivateRecurrenceScheduleResult(bool IsSuccess);
}
```

**Arquivo:** `Financial.Application/Features/RecurrenceSchedules/Commands/Deactivate/DeactivateRecurrenceScheduleCommandHandler.cs`

```csharp
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Financial.Application.Contracts;

namespace Financial.Application.Features.RecurrenceSchedules.Commands.Deactivate
{
    public class DeactivateRecurrenceScheduleCommandHandler
        : ICommandHandler<DeactivateRecurrenceScheduleCommand, DeactivateRecurrenceScheduleResult>
    {
        private readonly IRecurrenceScheduleService _service;

        public DeactivateRecurrenceScheduleCommandHandler(IRecurrenceScheduleService service)
        {
            _service = service;
        }

        public async Task<Result<DeactivateRecurrenceScheduleResult>> Handle(
            DeactivateRecurrenceScheduleCommand command, CancellationToken cancellationToken)
        {
            var result = await _service.DeactivateAsync(command.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<DeactivateRecurrenceScheduleResult>(result.Error!);

            return Result.Success(new DeactivateRecurrenceScheduleResult(result.Value!));
        }
    }
}
```

#### Delete

**Arquivo:** `Financial.Application/Features/RecurrenceSchedules/Commands/Delete/DeleteRecurrenceScheduleCommand.cs`

```csharp
using BuildingBlocks.CQRS.Requests.Commands;

namespace Financial.Application.Features.RecurrenceSchedules.Commands.Delete
{
    public record DeleteRecurrenceScheduleCommand(int Id)
        : ICommand<DeleteRecurrenceScheduleResult>;

    public record DeleteRecurrenceScheduleResult(bool IsSuccess);
}
```

**Arquivo:** `Financial.Application/Features/RecurrenceSchedules/Commands/Delete/DeleteRecurrenceScheduleCommandHandler.cs`

```csharp
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Financial.Application.Contracts;

namespace Financial.Application.Features.RecurrenceSchedules.Commands.Delete
{
    public class DeleteRecurrenceScheduleCommandHandler
        : ICommandHandler<DeleteRecurrenceScheduleCommand, DeleteRecurrenceScheduleResult>
    {
        private readonly IRecurrenceScheduleService _service;

        public DeleteRecurrenceScheduleCommandHandler(IRecurrenceScheduleService service)
        {
            _service = service;
        }

        public async Task<Result<DeleteRecurrenceScheduleResult>> Handle(
            DeleteRecurrenceScheduleCommand command, CancellationToken cancellationToken)
        {
            var result = await _service.DeleteAsync(command.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<DeleteRecurrenceScheduleResult>(result.Error!);

            return Result.Success(new DeleteRecurrenceScheduleResult(result.Value!));
        }
    }
}
```

### 2.8 Queries CQRS — RecurrenceSchedule

#### GetById

**Arquivo:** `Financial.Application/Features/RecurrenceSchedules/Queries/GetById/GetRecurrenceScheduleByIdQuery.cs`

```csharp
using BuildingBlocks.CQRS.Requests.Queries;
using Financial.Application.DTOs.RecurrenceSchedule;

namespace Financial.Application.Features.RecurrenceSchedules.Queries.GetById
{
    public record GetRecurrenceScheduleByIdQuery(int Id)
        : IQuery<GetRecurrenceScheduleByIdResult>;

    public record GetRecurrenceScheduleByIdResult(RecurrenceScheduleDTO Schedule);
}
```

**Arquivo:** `Financial.Application/Features/RecurrenceSchedules/Queries/GetById/GetRecurrenceScheduleByIdQueryHandler.cs`

```csharp
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Financial.Application.Contracts;

namespace Financial.Application.Features.RecurrenceSchedules.Queries.GetById
{
    public class GetRecurrenceScheduleByIdQueryHandler
        : IQueryHandler<GetRecurrenceScheduleByIdQuery, GetRecurrenceScheduleByIdResult>
    {
        private readonly IRecurrenceScheduleService _service;

        public GetRecurrenceScheduleByIdQueryHandler(IRecurrenceScheduleService service)
        {
            _service = service;
        }

        public async Task<Result<GetRecurrenceScheduleByIdResult>> Handle(
            GetRecurrenceScheduleByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _service.GetByIdAsync(query.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetRecurrenceScheduleByIdResult>(result.Error!);

            return Result.Success(new GetRecurrenceScheduleByIdResult(result.Value!));
        }
    }
}
```

#### GetByUserId

**Arquivo:** `Financial.Application/Features/RecurrenceSchedules/Queries/GetByUserId/GetRecurrenceSchedulesByUserIdQuery.cs`

```csharp
using BuildingBlocks.CQRS.Requests.Queries;
using Financial.Application.DTOs.RecurrenceSchedule;

namespace Financial.Application.Features.RecurrenceSchedules.Queries.GetByUserId
{
    public record GetRecurrenceSchedulesByUserIdQuery(int UserId)
        : IQuery<GetRecurrenceSchedulesByUserIdResult>;

    public record GetRecurrenceSchedulesByUserIdResult(
        IEnumerable<RecurrenceScheduleDTO> Schedules);
}
```

**Arquivo:** `Financial.Application/Features/RecurrenceSchedules/Queries/GetByUserId/GetRecurrenceSchedulesByUserIdQueryHandler.cs`

```csharp
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Financial.Application.Contracts;

namespace Financial.Application.Features.RecurrenceSchedules.Queries.GetByUserId
{
    public class GetRecurrenceSchedulesByUserIdQueryHandler
        : IQueryHandler<GetRecurrenceSchedulesByUserIdQuery, GetRecurrenceSchedulesByUserIdResult>
    {
        private readonly IRecurrenceScheduleService _service;

        public GetRecurrenceSchedulesByUserIdQueryHandler(IRecurrenceScheduleService service)
        {
            _service = service;
        }

        public async Task<Result<GetRecurrenceSchedulesByUserIdResult>> Handle(
            GetRecurrenceSchedulesByUserIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _service.GetActiveByUserIdAsync(query.UserId, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetRecurrenceSchedulesByUserIdResult>(result.Error!);

            return Result.Success(new GetRecurrenceSchedulesByUserIdResult(result.Value!));
        }
    }
}
```

#### GetByFilter

**Arquivo:** `Financial.Application/Features/RecurrenceSchedules/Queries/GetByFilter/GetRecurrenceSchedulesByFilterQuery.cs`

```csharp
using BuildingBlocks.CQRS.Requests.Queries;
using BuildingBlocks.Results;
using Financial.Application.DTOs.RecurrenceSchedule;

namespace Financial.Application.Features.RecurrenceSchedules.Queries.GetByFilter
{
    public record GetRecurrenceSchedulesByFilterQuery(RecurrenceScheduleFilterDTO Filter)
        : IQuery<GetRecurrenceSchedulesByFilterResult>;

    public record GetRecurrenceSchedulesByFilterResult(
        IEnumerable<RecurrenceScheduleDTO> Items,
        PaginationData Pagination);
}
```

**Arquivo:** `Financial.Application/Features/RecurrenceSchedules/Queries/GetByFilter/GetRecurrenceSchedulesByFilterQueryHandler.cs`

```csharp
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Financial.Application.Contracts;

namespace Financial.Application.Features.RecurrenceSchedules.Queries.GetByFilter
{
    public class GetRecurrenceSchedulesByFilterQueryHandler
        : IQueryHandler<GetRecurrenceSchedulesByFilterQuery, GetRecurrenceSchedulesByFilterResult>
    {
        private readonly IRecurrenceScheduleService _service;

        public GetRecurrenceSchedulesByFilterQueryHandler(IRecurrenceScheduleService service)
        {
            _service = service;
        }

        public async Task<Result<GetRecurrenceSchedulesByFilterResult>> Handle(
            GetRecurrenceSchedulesByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _service.GetByFilterAsync(query.Filter, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetRecurrenceSchedulesByFilterResult>(result.Error!);

            return Result.Success(new GetRecurrenceSchedulesByFilterResult(
                result.Value!.Items, result.Value.Pagination));
        }
    }
}
```

---

## 3. Infrastructure Layer

### 3.1 Entity Configuration — `RecurrenceScheduleConfiguration`

**Arquivo:** `Financial.Infrastructure/Configuration/RecurrenceScheduleConfiguration.cs`

```csharp
using Financial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Financial.Infrastructure.Configuration
{
    public class RecurrenceScheduleConfiguration : IEntityTypeConfiguration<RecurrenceSchedule>
    {
        public void Configure(EntityTypeBuilder<RecurrenceSchedule> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Frequency).IsRequired();
            builder.Property(r => r.StartDate).IsRequired();
            builder.Property(r => r.IsActive).HasDefaultValue(true);
            builder.Property(r => r.OccurrencesGenerated).HasDefaultValue(0);

            // Relação 1:1 com Transaction
            builder.HasOne(r => r.SourceTransaction)
                .WithOne(t => t.RecurrenceSchedule)
                .HasForeignKey<RecurrenceSchedule>(r => r.SourceTransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(r => r.SourceTransactionId).IsUnique();
            builder.HasIndex(r => r.IsActive);
            builder.HasIndex(r => r.NextOccurrence);
            builder.HasIndex(r => new { r.IsActive, r.NextOccurrence });
        }
    }
}
```

### 3.2 DbContext — Adicionar DbSet

**Edição em:** `Financial.Infrastructure/Persistence/ApplicationDbContext.cs`

```csharp
public DbSet<RecurrenceSchedule> RecurrenceSchedules { get; set; }
```

### 3.3 Repository — `RecurrenceScheduleRepository`

**Arquivo:** `Financial.Infrastructure/Persistence/Repositories/RecurrenceScheduleRepository.cs`

```csharp
using Financial.Domain.Entities;
using Financial.Domain.Filters;
using Financial.Domain.Filters.Specifications;
using Financial.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Financial.Infrastructure.Persistence.Repositories
{
    public class RecurrenceScheduleRepository : IRecurrenceScheduleRepository
    {
        private readonly ApplicationDbContext _context;

        public RecurrenceScheduleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Create(RecurrenceSchedule entity, CancellationToken cancellationToken = default)
        {
            await _context.RecurrenceSchedules.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<RecurrenceSchedule> entities, CancellationToken cancellationToken = default)
        {
            await _context.RecurrenceSchedules.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(RecurrenceSchedule entity, CancellationToken cancellationToken = default)
        {
            entity.Deactivate();
            _context.RecurrenceSchedules.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<RecurrenceSchedule?>> Find(
            Expression<Func<RecurrenceSchedule, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _context.RecurrenceSchedules
                .Include(r => r.SourceTransaction)
                    .ThenInclude(t => t.Category)
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }

        public async Task<(IEnumerable<RecurrenceSchedule> Items, int TotalCount)> FindByFilter(
            RecurrenceScheduleQueryFilter filter,
            CancellationToken cancellationToken = default)
        {
            var specification = new RecurrenceScheduleSpecification(filter);
            var query = _context.RecurrenceSchedules
                .Include(r => r.SourceTransaction)
                    .ThenInclude(t => t.Category)
                .Where(specification.ToExpression());

            var totalCount = await query.CountAsync(cancellationToken);

            var page = filter.Page ?? 1;
            var pageSize = filter.PageSize ?? 50;

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<IEnumerable<RecurrenceSchedule?>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _context.RecurrenceSchedules
                .Include(r => r.SourceTransaction)
                    .ThenInclude(t => t.Category)
                .ToListAsync(cancellationToken);
        }

        public async Task<RecurrenceSchedule?> GetById(int id, CancellationToken cancellationToken = default)
        {
            return await _context.RecurrenceSchedules
                .Include(r => r.SourceTransaction)
                    .ThenInclude(t => t.Category)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task Update(RecurrenceSchedule entity, CancellationToken cancellationToken = default)
        {
            _context.RecurrenceSchedules.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<RecurrenceSchedule?> GetBySourceTransactionIdAsync(
            int sourceTransactionId, CancellationToken cancellationToken = default)
        {
            return await _context.RecurrenceSchedules
                .Include(r => r.SourceTransaction)
                    .ThenInclude(t => t.Category)
                .FirstOrDefaultAsync(r => r.SourceTransactionId == sourceTransactionId, cancellationToken);
        }

        public async Task<IEnumerable<RecurrenceSchedule>> GetActiveByUserIdAsync(
            int userId, CancellationToken cancellationToken = default)
        {
            return await _context.RecurrenceSchedules
                .Include(r => r.SourceTransaction)
                    .ThenInclude(t => t.Category)
                .Where(r => r.SourceTransaction.UserId == userId && r.IsActive && !r.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<RecurrenceSchedule>> GetDueSchedulesAsync(
            DateTime referenceDate, CancellationToken cancellationToken = default)
        {
            return await _context.RecurrenceSchedules
                .Include(r => r.SourceTransaction)
                    .ThenInclude(t => t.Category)
                .Where(r => r.IsActive && !r.IsDeleted && r.NextOccurrence <= referenceDate)
                .ToListAsync(cancellationToken);
        }
    }
}
```

### 3.4 Service — `RecurrenceScheduleService`

**Arquivo:** `Financial.Infrastructure/Services/RecurrenceScheduleService.cs`

```csharp
using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.RecurrenceSchedule;
using Financial.Application.Mappings;
using Financial.Domain.Errors;
using Financial.Domain.Filters;
using Financial.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Financial.Infrastructure.Services
{
    public class RecurrenceScheduleService : IRecurrenceScheduleService
    {
        private readonly IRecurrenceScheduleRepository _repository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<RecurrenceScheduleService> _logger;

        public RecurrenceScheduleService(
            IRecurrenceScheduleRepository repository,
            ITransactionRepository transactionRepository,
            ILogger<RecurrenceScheduleService> logger)
        {
            _repository = repository;
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<Result<int>> CreateAsync(
            CreateRecurrenceScheduleDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<int>(RecurrenceScheduleErrors.CreateError);

                // Verificar se a transação de origem existe
                var sourceTransaction = await _transactionRepository.GetById(
                    dto.SourceTransactionId, cancellationToken);
                if (sourceTransaction == null)
                    return Result.Failure<int>(RecurrenceScheduleErrors.InvalidSourceTransaction);

                if (!sourceTransaction.IsRecurring)
                    return Result.Failure<int>(RecurrenceScheduleErrors.SourceTransactionNotRecurring);

                // Verificar se já existe schedule para esta transação
                var existing = await _repository.GetBySourceTransactionIdAsync(
                    dto.SourceTransactionId, cancellationToken);
                if (existing != null)
                    return Result.Failure<int>(RecurrenceScheduleErrors.ScheduleAlreadyExists);

                var entity = RecurrenceScheduleMapper.ToEntity(dto);
                await _repository.Create(entity, cancellationToken);

                return Result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar agendamento {@Data}", dto);
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateAsync(
            UpdateRecurrenceScheduleDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(Error.NullValue);

                if (dto.Id <= 0)
                    return Result.Failure<bool>(RecurrenceScheduleErrors.InvalidId);

                var entity = await _repository.GetById(dto.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(RecurrenceScheduleErrors.NotFound(dto.Id));

                entity.Update(dto.Frequency, dto.EndDate, dto.MaxOccurrences);
                await _repository.Update(entity, cancellationToken);

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar agendamento {@Data}", dto);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return Result.Failure<bool>(RecurrenceScheduleErrors.InvalidId);

                var entity = await _repository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(RecurrenceScheduleErrors.NotFound(id));

                await _repository.Delete(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir agendamento {Id}", id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<RecurrenceScheduleDTO>> GetByIdAsync(
            int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return Result.Failure<RecurrenceScheduleDTO>(RecurrenceScheduleErrors.InvalidId);

                var entity = await _repository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<RecurrenceScheduleDTO>(RecurrenceScheduleErrors.NotFound(id));

                return Result.Success(RecurrenceScheduleMapper.ToDTO(entity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar agendamento {Id}", id);
                return Result.Failure<RecurrenceScheduleDTO>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<RecurrenceScheduleDTO>>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _repository.GetAll(cancellationToken);
                var dtos = all.Select(RecurrenceScheduleMapper.ToDTO).ToList();
                return Result.Success<IEnumerable<RecurrenceScheduleDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os agendamentos");
                return Result.Failure<IEnumerable<RecurrenceScheduleDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<RecurrenceScheduleDTO?>> GetBySourceTransactionIdAsync(
            int sourceTransactionId, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _repository.GetBySourceTransactionIdAsync(
                    sourceTransactionId, cancellationToken);
                if (entity == null)
                    return Result.Success<RecurrenceScheduleDTO?>(null);

                return Result.Success<RecurrenceScheduleDTO?>(RecurrenceScheduleMapper.ToDTO(entity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar agendamento da transação {Id}", sourceTransactionId);
                return Result.Failure<RecurrenceScheduleDTO?>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<RecurrenceScheduleDTO>>> GetActiveByUserIdAsync(
            int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (userId <= 0)
                    return Result.Failure<IEnumerable<RecurrenceScheduleDTO>>(
                        RecurrenceScheduleErrors.InvalidId);

                var entities = await _repository.GetActiveByUserIdAsync(userId, cancellationToken);
                var dtos = entities.Select(RecurrenceScheduleMapper.ToDTO).ToList();
                return Result.Success<IEnumerable<RecurrenceScheduleDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar agendamentos do usuário {UserId}", userId);
                return Result.Failure<IEnumerable<RecurrenceScheduleDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeactivateAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _repository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(RecurrenceScheduleErrors.NotFound(id));

                entity.Deactivate();
                await _repository.Update(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar agendamento {Id}", id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> ActivateAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _repository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(RecurrenceScheduleErrors.NotFound(id));

                entity.Activate();
                await _repository.Update(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao ativar agendamento {Id}", id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        /// <summary>
        /// Processa todos os schedules pendentes até a data atual,
        /// copiando os dados da Transaction original para gerar novas transações.
        /// </summary>
        public async Task<Result<int>> ProcessDueSchedulesAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var now = DateTime.UtcNow;
                var dueSchedules = await _repository.GetDueSchedulesAsync(now, cancellationToken);
                var transactionsCreated = 0;

                foreach (var schedule in dueSchedules)
                {
                    while (schedule.CanGenerateNext() && schedule.NextOccurrence <= now)
                    {
                        var transaction = schedule.GenerateTransaction();
                        await _transactionRepository.Create(transaction, cancellationToken);
                        transactionsCreated++;
                    }

                    await _repository.Update(schedule, cancellationToken);
                }

                _logger.LogInformation(
                    "Processamento concluído. {Count} transações criadas.", transactionsCreated);

                return Result.Success(transactionsCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar agendamentos pendentes");
                return Result.Failure<int>(RecurrenceScheduleErrors.ProcessingError);
            }
        }

        public async Task<Result<(IEnumerable<RecurrenceScheduleDTO> Items, PaginationData Pagination)>> GetByFilterAsync(
            RecurrenceScheduleFilterDTO filter, CancellationToken cancellationToken = default)
        {
            try
            {
                var domainFilter = new RecurrenceScheduleQueryFilter(
                    filter.Id, filter.SourceTransactionId, filter.UserId,
                    filter.Frequency, filter.IsActive,
                    filter.StartDateFrom, filter.StartDateTo,
                    filter.CreatedAt, filter.UpdatedAt, filter.IsDeleted,
                    filter.SortBy, filter.SortDesc, filter.Page, filter.PageSize);

                var (entities, totalItems) = await _repository.FindByFilter(domainFilter, cancellationToken);
                if (!entities.Any())
                    return Result.Success<(IEnumerable<RecurrenceScheduleDTO> Items, PaginationData Pagination)>(
                        (new List<RecurrenceScheduleDTO>(), new PaginationData(filter.Page, filter.PageSize)));

                var dtos = entities
                    .Where(e => e != null)
                    .Select(RecurrenceScheduleMapper.ToDTO)
                    .ToList();

                var pageSize = filter.PageSize ?? 50;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                var pagination = new PaginationData(filter.Page, pageSize, totalItems, totalPages);

                return Result.Success<(IEnumerable<RecurrenceScheduleDTO> Items, PaginationData Pagination)>(
                    (dtos, pagination));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar agendamentos com filtro {@Filter}", filter);
                return Result.Failure<(IEnumerable<RecurrenceScheduleDTO>, PaginationData)>(
                    Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CountAsync(
            Expression<Func<RecurrenceScheduleDTO, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _repository.GetAll(cancellationToken);
                return Result.Success(all.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar agendamentos");
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(
            IEnumerable<CreateRecurrenceScheduleDTO> dtos,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                    return Result.Failure<IEnumerable<int>>(Error.NullValue);

                var entities = dtos.Select(RecurrenceScheduleMapper.ToEntity).ToList();
                await _repository.CreateRange(entities, cancellationToken);
                return Result.Success<IEnumerable<int>>(entities.Select(e => e.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar múltiplos agendamentos");
                return Result.Failure<IEnumerable<int>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(
            IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result.Failure<bool>(Error.NullValue);

                foreach (var id in ids)
                {
                    var entity = await _repository.GetById(id, cancellationToken);
                    if (entity != null)
                        await _repository.Delete(entity, cancellationToken);
                }
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir múltiplos agendamentos");
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<RecurrenceScheduleDTO>>> FindAsync(
            Expression<Func<RecurrenceScheduleDTO, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _repository.GetAll(cancellationToken);
                var dtos = all
                    .Select(RecurrenceScheduleMapper.ToDTO)
                    .AsQueryable()
                    .Where(predicate)
                    .ToList();
                return Result.Success<IEnumerable<RecurrenceScheduleDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar agendamentos com predicado");
                return Result.Failure<IEnumerable<RecurrenceScheduleDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<RecurrenceScheduleDTO> Items, int TotalCount)>> GetPagedAsync(
            int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _repository.GetAll(cancellationToken);
                var totalCount = all.Count();
                var items = all
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(RecurrenceScheduleMapper.ToDTO)
                    .ToList();
                return Result.Success<(IEnumerable<RecurrenceScheduleDTO> Items, int TotalCount)>(
                    (items, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter página de agendamentos");
                return Result.Failure<(IEnumerable<RecurrenceScheduleDTO>, int)>(Error.Failure(ex.Message));
            }
        }
    }
}
```

### 3.5 Background Service — `RecurrenceProcessorService`

**Arquivo:** `Financial.Infrastructure/BackgroundServices/RecurrenceProcessorService.cs`

```csharp
using Financial.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Financial.Infrastructure.BackgroundServices
{
    public class RecurrenceProcessorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RecurrenceProcessorService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromHours(1);

        public RecurrenceProcessorService(
            IServiceScopeFactory scopeFactory,
            ILogger<RecurrenceProcessorService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço de processamento de recorrências iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider
                        .GetRequiredService<IRecurrenceScheduleService>();

                    var result = await service.ProcessDueSchedulesAsync(stoppingToken);

                    if (result.IsSuccess)
                        _logger.LogInformation(
                            "Processamento concluído: {Count} transações geradas.", result.Value);
                    else
                        _logger.LogWarning(
                            "Falha no processamento: {Error}", result.Error?.Description);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado no processamento de recorrências.");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
```

### 3.6 Dependency Injection — Atualização

**Edição em:** `Financial.Infrastructure/DependencyInjection.cs`

```csharp
// Adicionar:
services.AddScoped<IRecurrenceScheduleRepository, RecurrenceScheduleRepository>();
services.AddScoped<IRecurrenceScheduleService, RecurrenceScheduleService>();
services.AddHostedService<RecurrenceProcessorService>();
```

### 3.7 Migration

```bash
dotnet ef migrations add AddRecurrenceSchedule \
    --project Services/Financial/Financial.Infrastructure \
    --startup-project Services/Financial/Financial.API
```

Tabela gerada:

```
recurrence_schedules
├── id (int, PK, auto-increment)
├── source_transaction_id (int, FK → transactions.id, UNIQUE, CASCADE)
├── frequency (int, NOT NULL)
├── start_date (timestamp with time zone, NOT NULL)
├── end_date (timestamp with time zone, nullable)
├── next_occurrence (timestamp with time zone, NOT NULL, indexed)
├── max_occurrences (int, nullable)
├── occurrences_generated (int, default 0)
├── is_active (bool, default true, indexed)
├── created_at (timestamp with time zone)
├── updated_at (timestamp with time zone, nullable)
└── is_deleted (bool)
```

---

## 4. API Layer

### 4.1 Controller — `RecurrenceSchedulesController`

**Arquivo:** `Financial.API/Controllers/RecurrenceSchedulesController.cs`

```csharp
using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Financial.Application.DTOs.RecurrenceSchedule;
using Financial.Application.Features.RecurrenceSchedules.Commands.Deactivate;
using Financial.Application.Features.RecurrenceSchedules.Commands.Delete;
using Financial.Application.Features.RecurrenceSchedules.Commands.Update;
using Financial.Application.Features.RecurrenceSchedules.Queries.GetByFilter;
using Financial.Application.Features.RecurrenceSchedules.Queries.GetById;
using Financial.Application.Features.RecurrenceSchedules.Queries.GetByUserId;
using Microsoft.AspNetCore.Mvc;

namespace Financial.API.Controllers
{
    public class RecurrenceSchedulesController : ApiController
    {
        private readonly ISender _sender;

        public RecurrenceSchedulesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<object>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var query = new GetRecurrenceScheduleByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!.Schedule)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<HttpResult<object>> GetActiveByUserIdAsync(
            int userId, CancellationToken cancellationToken)
        {
            var query = new GetRecurrenceSchedulesByUserIdQuery(userId);
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!.Schedules)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet("search")]
        public async Task<HttpResult<object>> Search(
            [FromQuery] RecurrenceScheduleFilterDTO filter,
            CancellationToken cancellationToken)
        {
            var query = new GetRecurrenceSchedulesByFilterQuery(filter);
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value.Items, result.Value.Pagination)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<object>> UpdateAsync(
            int id, [FromBody] UpdateRecurrenceScheduleCommand command,
            CancellationToken cancellationToken)
        {
            var updatedCommand = new UpdateRecurrenceScheduleCommand(
                id, command.Frequency, command.EndDate, command.MaxOccurrences);
            var result = await _sender.Send(updatedCommand, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!.IsSuccess)
                : result.Error!.Description.Contains("NotFound")
                    ? HttpResult<object>.NotFound(result.Error!.Description)
                    : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPatch("{id:int}/deactivate")]
        public async Task<HttpResult<object>> DeactivateAsync(
            int id, CancellationToken cancellationToken)
        {
            var command = new DeactivateRecurrenceScheduleCommand(id);
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!.IsSuccess)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<object>> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteRecurrenceScheduleCommand(id);
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Deleted()
                : HttpResult<object>.NotFound(result.Error!.Description);
        }
    }
}
```

---

## 5. Fluxo Completo do Usuário

```
┌─────────────────────────────────────────────────────────────┐
│                    FLUXO DE CRIAÇÃO                          │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  1. POST /api/transactions                                  │
│     {                                                       │
│       "userId": 1,                                          │
│       "categoryId": 5,                                      │
│       "paymentMethod": 5,                                   │
│       "transactionType": 2,                                 │
│       "amount": { "amount": 150000, "currency": 13 },      │
│       "description": "Aluguel",                             │
│       "transactionDate": "2026-04-01T00:00:00Z",           │
│       "isRecurring": true,          ◄── marca como recorr. │
│       "frequency": 3,               ◄── mensal             │
│       "recurrenceEndDate": "2027-03-31T00:00:00Z"          │
│     }                                                       │
│                                                             │
│  2. CreateTransactionCommandHandler:                        │
│     ├─ Cria Transaction (IsRecurring=true)                  │
│     └─ Se IsRecurring && Frequency != null:                 │
│        └─ Cria RecurrenceSchedule vinculado                 │
│                                                             │
│  3. Background Service (a cada 1h):                         │
│     ├─ Busca schedules com NextOccurrence <= agora          │
│     ├─ Para cada schedule:                                  │
│     │   ├─ Lê SourceTransaction (dados originais)           │
│     │   ├─ Copia dados → nova Transaction                   │
│     │   ├─ Avança NextOccurrence                            │
│     │   └─ Desativa se atingiu limite                       │
│     └─ Salva tudo                                           │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## 6. Endpoints REST

### Transactions (existente — alterado)

| Método | Rota | Mudança |
|--------|------|---------|
| `POST` | `/api/transactions` | Aceita campos `frequency`, `recurrenceEndDate`, `maxOccurrences` |

### RecurrenceSchedules (novo)

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/recurrence-schedules/{id}` | Busca schedule por ID |
| `GET` | `/api/recurrence-schedules/user/{userId}` | Lista schedules ativos do usuário |
| `GET` | `/api/recurrence-schedules/search` | Busca com filtros e paginação |
| `PUT` | `/api/recurrence-schedules/{id}` | Atualiza frequência/datas |
| `PATCH` | `/api/recurrence-schedules/{id}/deactivate` | Desativa recorrência |
| `DELETE` | `/api/recurrence-schedules/{id}` | Remove recorrência |

> **Nota:** Não há `POST` em recurrence-schedules — a criação é feita automaticamente via `POST /api/transactions` quando `isRecurring = true`.

---

## 7. Exemplos de Requisições

### Criar transação recorrente (aluguel mensal)

```json
POST /api/transactions
{
    "userId": 1,
    "categoryId": 5,
    "paymentMethod": 4,
    "transactionType": 2,
    "amount": { "amount": 150000, "currency": 13 },
    "description": "Aluguel do apartamento",
    "transactionDate": "2026-04-01T00:00:00Z",
    "isRecurring": true,
    "frequency": 3,
    "recurrenceEndDate": "2027-03-31T00:00:00Z"
}
```

### Criar transação NÃO recorrente (compra normal)

```json
POST /api/transactions
{
    "userId": 1,
    "categoryId": 2,
    "paymentMethod": 2,
    "transactionType": 2,
    "amount": { "amount": 5990, "currency": 13 },
    "description": "Almoço restaurante",
    "transactionDate": "2026-03-10T12:00:00Z",
    "isRecurring": false
}
```

### Criar internet mensal com limite

```json
POST /api/transactions
{
    "userId": 1,
    "categoryId": 8,
    "paymentMethod": 5,
    "transactionType": 2,
    "amount": { "amount": 12990, "currency": 13 },
    "description": "Internet fibra",
    "transactionDate": "2026-04-01T00:00:00Z",
    "isRecurring": true,
    "frequency": 3,
    "maxOccurrences": 12
}
```

### Criar transporte diário

```json
POST /api/transactions
{
    "userId": 1,
    "categoryId": 3,
    "paymentMethod": 1,
    "transactionType": 2,
    "amount": { "amount": 1500, "currency": 13 },
    "description": "Transporte diário",
    "transactionDate": "2026-04-01T00:00:00Z",
    "isRecurring": true,
    "frequency": 1,
    "recurrenceEndDate": "2026-12-31T00:00:00Z"
}
```

### Alterar frequência de recorrência

```json
PUT /api/recurrence-schedules/5
{
    "frequency": 2,
    "endDate": "2026-12-31T00:00:00Z"
}
```

### Buscar recorrências ativas do usuário

```
GET /api/recurrence-schedules/user/1
```

### Desativar recorrência

```
PATCH /api/recurrence-schedules/5/deactivate
```

---

## 8. Checklist de Implementação

### Domain Layer
- [ ] `RecurrenceFrequency` enum
- [ ] `RecurrenceSchedule` entity
- [ ] `RecurrenceScheduleErrors` errors
- [ ] `RecurrenceScheduleQueryFilter` filter
- [ ] `RecurrenceScheduleSpecification` specification
- [ ] `IRecurrenceScheduleRepository` interface
- [ ] Adicionar `RecurrenceSchedule?` navigation em `Transaction`

### Application Layer
- [ ] DTOs (Create, Update, Response, Filter)
- [ ] `RecurrenceScheduleMapper`
- [ ] `IRecurrenceScheduleService` contract
- [ ] **Alterar** `CreateTransactionCommand` (adicionar campos de recorrência)
- [ ] **Alterar** `CreateTransactionCommandValidator` (validação condicional)
- [ ] **Alterar** `CreateTransactionCommandHandler` (criar schedule junto)
- [ ] Commands: Update, Delete, Deactivate (+ Handlers)
- [ ] Queries: GetById, GetByUserId, GetByFilter (+ Handlers)

### Infrastructure Layer
- [ ] `RecurrenceScheduleConfiguration` (EF Core)
- [ ] Adicionar `DbSet<RecurrenceSchedule>` ao `ApplicationDbContext`
- [ ] `RecurrenceScheduleRepository`
- [ ] `RecurrenceScheduleService`
- [ ] `RecurrenceProcessorService` (Background Service)
- [ ] Registrar DI (repository, service, hosted service)
- [ ] Migration: `AddRecurrenceSchedule`

### API Layer
- [ ] `RecurrenceSchedulesController`

### Testes
- [ ] Testes unitários: `RecurrenceSchedule` entity (geração, limites, ativação)
- [ ] Testes unitários: `CreateTransactionCommandHandler` (com e sem recorrência)
- [ ] Testes de integração: `RecurrenceScheduleRepository`
- [ ] Testes E2E: endpoints de criação com recorrência
