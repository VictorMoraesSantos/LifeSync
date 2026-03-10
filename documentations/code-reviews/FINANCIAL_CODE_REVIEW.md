# Financial Microservice - Code Review Completo

> **Autor:** Code Review Senior
> **Data:** 03/03/2026
> **Escopo:** Varredura completa de todas as camadas (Domain, Application, Infrastructure, API)
> **Severidade:** CRITICO | ALTO | MEDIO | BAIXO | INFO

---

## Sumario Executivo

O microservice Financial gerencia categorias e transacoes financeiras com suporte a Value Object (Money), CQRS e Result Pattern. A arquitetura segue Clean Architecture, porem apresenta **bugs criticos no domain model**, **problemas graves de performance**, **falhas de seguranca** e **testes de integracao ausentes**.

### Nota Geral: 5.5/10

---

## 1. DOMAIN LAYER

### 1.1 [CRITICO] Bug Dupla Atribuicao no Transaction.Update()

**Arquivo:** `Transaction.cs`

```csharp
private void SetAmout(Money amount) // TYPO: "Amout" ao inves de "Amount"
{
    if (amount == null)
        throw new DomainException(TransactionErrors.InvalidAmount);
    Amount = amount; // Primeira atribuicao
}

public void Update(...)
{
    SetAmout(amount);
    // ...
    Amount = amount; // Segunda atribuicao - REDUNDANTE!
}
```

**Impacto:** O `Amount` e atribuido duas vezes. O setter privado perde o proposito. Alem disso, o nome do metodo tem typo.

**Correcao:**
1. Renomear `SetAmout` para `SetAmount`
2. Remover a atribuicao direta `Amount = amount` do `Update()`

---

### 1.2 [CRITICO] Money Value Object - Contradicao de Regra de Negocio

**Arquivo:** `Money.cs`

```csharp
public static Money Create(decimal amount, string currency)
{
    if (amount < 0)
        throw new ArgumentException("Amount must be non-negative");
}
```

O Money nao permite valores negativos, porem o dominio financeiro requer representacao de despesas (valores negativos).

**Impacto:** Impossivel representar despesas se o Amount deve ser sempre positivo. Se a logica depende do `TransactionType` para indicar debito/credito, o design nao esta documentado.

**Correcao:** Documentar claramente a estrategia (Amount sempre positivo + Type indica direcao) ou permitir negativos.

---

### 1.3 [ALTO] Inconsistencia de Excecoes no Domain

**Arquivos:** `Category.cs`, `Transaction.cs`

```csharp
// Category usa DomainException:
if (userId <= 0)
    throw new DomainException(CategoryErrors.InvalidUserId);

// Transaction usa ArgumentOutOfRangeException:
ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);
```

**Impacto:** Dois padroes diferentes de validacao no mesmo dominio. Handlers que capturam `Exception` generica mascaram o tipo real.

**Correcao:** Padronizar para `DomainException` em todas as entidades.

---

### 1.4 [ALTO] Validacao Logica Incorreta no CreateTransactionCommandValidator

**Arquivo:** `CreateTransactionCommandValidator.cs`

```csharp
.Must(amount => amount.Amount > 0 || amount.Amount < 0)
```

Esta expressao SEMPRE retorna true (qualquer numero e positivo OU negativo). Deveria ser:

```csharp
.Must(amount => amount.Amount != 0)
```

---

### 1.5 [MEDIO] Erros Tipados Comparados por String no Controller

**Arquivo:** `CategoriesController.cs`

```csharp
: result.Error!.Description.Contains("NotFound")
    ? HttpResult<object>.NotFound(...)
```

**Impacto:** Comparacao por string e fragil. Se a mensagem mudar, o mapeamento de status HTTP quebra.

**Correcao:** Usar `result.Error.Type == ErrorType.NotFound`.

---

## 2. APPLICATION LAYER

### 2.1 [ALTO] Handlers Sao Apenas Pass-Through

Todos os handlers seguem o mesmo padrao sem logica adicional:

```csharp
public async Task<Result<CreateCategoryResult>> Handle(...)
{
    var dto = new CreateCategoryDTO(...);
    var result = await _categoryService.CreateAsync(dto, cancellationToken);
    if (!result.IsSuccess)
        return Result<CreateCategoryResult>.Failure(result.Error!);
    return Result.Success(new CreateCategoryResult(result.Value!));
}
```

**Impacto:** Camada adicional sem valor. Os handlers poderiam conter a logica diretamente ou serem eliminados em favor dos services.

---

### 2.2 [MEDIO] DTOs Nao Utilizados

`AccountBalanceDTO` e `UserBalanceSummaryDTO` estao definidos mas nunca referenciados.

**Correcao:** Remover ou implementar funcionalidade de balanco.

---

### 2.3 [MEDIO] ReportsController Vazio

O controller de relatorios esta como stub vazio.

**Correcao:** Implementar ou remover para nao confundir.

---

## 3. INFRASTRUCTURE LAYER

### 3.1 [CRITICO] Paginacao Em Memoria

**Arquivo:** `CategoryService.cs`

```csharp
var entities = await _categoryRepository.GetAll(cancellationToken); // CARREGA TUDO
var dtos = entities.Select(CategoryMapper.ToDTO)
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToList();
```

**Impacto:** Identico ao problema do TaskManager. Com 100k registros, todos sao carregados em memoria.

**Correcao:** Paginar no banco via `IQueryable.Skip().Take()`.

---

### 3.2 [CRITICO] FindAsync Carrega Tudo Em Memoria

```csharp
public async Task<Result<IEnumerable<CategoryDTO>>> FindAsync(
    Expression<Func<CategoryDTO, bool>> predicate, ...)
{
    var entities = await _categoryRepository.GetAll(cancellationToken);
    var dtos = entities
        .Select(CategoryMapper.ToDTO)
        .AsQueryable()
        .Where(predicate) // Filtragem EM MEMORIA sobre DTOs!
        .ToList();
}
```

**Impacto:** Carrega TODOS os registros, converte para DTO, depois filtra. Performance O(n) sempre.

**Correcao:** Traduzir predicado para Expression<Func<Entity, bool>> e executar no banco.

---

### 3.3 [CRITICO] N+1 no DeleteRangeAsync

```csharp
foreach (var id in ids)
{
    var entity = await _categoryRepository.GetById(id, cancellationToken); // N queries!
}
```

**Correcao:** Criar metodo `GetByIds(IEnumerable<int> ids)` no repository.

---

### 3.4 [ALTO] Bug de Seguranca - GetByNameAsync Ignora UserId

**Arquivo:** `CategoryService.cs`

```csharp
public async Task<Result<IEnumerable<CategoryDTO>>> GetByNameAsync(
    string name, int userId, ...)
{
    // userId e VALIDADO mas NUNCA USADO na query!
    var entities = await _categoryRepository.GetByNameContains(name, cancellationToken);
}
```

**Impacto:** Retorna categorias de TODOS os usuarios que contenham o nome buscado. Vazamento de dados.

**Correcao:** Filtrar por `userId` na query.

---

### 3.5 [ALTO] Ausencia de Indexes no Banco

Apenas `CategoryId` em Transaction tem index. Faltam:

| Index Necessario | Justificativa |
|-----------------|---------------|
| `Categories.UserId` | Filtro por usuario |
| `Transactions.UserId` | Filtro por usuario |
| `Transactions.TransactionDate` | Filtro por periodo |
| `Transactions.PaymentMethod` | Filtro por metodo |

---

### 3.6 [ALTO] Ausencia de Global Query Filter para Soft Delete

`IsDeleted` existe nas entidades mas nao ha filtro global:

```csharp
// AUSENTE:
modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
```

**Impacto:** Registros deletados aparecem em todas as queries.

---

### 3.7 [ALTO] TransactionDTO com Category Nullable Incorreto

**Arquivo:** `TransactionMapper.cs`

```csharp
entity.Category != null ? CategoryMapper.ToDTO(entity.Category) : null
```

Porem `TransactionDTO` declara `Category` como nao-nullable. Pode causar `NullReferenceException`.

---

### 3.8 [MEDIO] Money OwnsOne Sem Precisao Decimal

```csharp
builder.OwnsOne(o => o.Amount, amountBuilder =>
{
    amountBuilder.Property(m => m.Amount);
    amountBuilder.Property(m => m.Currency);
});
```

**Impacto:** Sem `HasPrecision()`, o EF Core usa default (18,2). Para valores financeiros, deveria ser (18,4) ou mais.

**Correcao:**
```csharp
amountBuilder.Property(m => m.Amount).HasPrecision(18, 4);
```

---

## 4. API LAYER

### 4.1 [CRITICO] Sem [Authorize] nos Controllers

Nenhum controller possui `[Authorize]`. Todos os endpoints estao publicos.

---

### 4.2 [CRITICO] JWT Secret Hardcoded

```json
"Key": "SuperSecretKeyForJWTAuthentication2024!@#$%"
```

---

### 4.3 [ALTO] GetAll Retorna Dados de Todos os Usuarios

```csharp
var result = await _categoryService.GetAllAsync(cancellationToken); // TODOS os usuarios
```

**Correcao:** Filtrar por `UserId` extraido do JWT.

---

## 5. TESTES

### 5.1 [ALTO] Testes de Integracao Ausentes

Apenas stub vazio `UnitTest1` com `Test1()` vazio. Nenhum teste de integracao implementado.

### 5.2 [INFO] Testes Unitarios Bons

- CategoryTests: 12 casos cobrindo construtor e Update()
- TransactionTests: 20+ casos cobrindo construtor e Update()
- Sem FluentAssertions (usa Assert. nativo)

---

## 6. PLANO DE ACAO PRIORIZADO

### Prioridade 1 - Criticos
| # | Item | Esforco |
|---|------|---------|
| 1 | Adicionar [Authorize] nos controllers | 10 min |
| 2 | Remover JWT Key do appsettings.json | 30 min |
| 3 | Corrigir paginacao em memoria | 2h |
| 4 | Corrigir FindAsync para filtrar no banco | 2h |
| 5 | Corrigir N+1 no DeleteRangeAsync | 1h |
| 6 | Corrigir bug dupla atribuicao no Transaction.Update() | 15 min |
| 7 | Corrigir validador Must() sempre true | 10 min |

### Prioridade 2 - Altos
| # | Item | Esforco |
|---|------|---------|
| 8 | Filtrar GetByNameAsync por UserId | 30 min |
| 9 | Adicionar indexes no banco | 30 min |
| 10 | Adicionar Global Query Filter para IsDeleted | 30 min |
| 11 | Corrigir TransactionDTO nullability | 30 min |
| 12 | Filtrar GetAll por usuario autenticado | 1h |
| 13 | Padronizar excecoes de dominio | 1h |
| 14 | Implementar testes de integracao | 8h |

### Prioridade 3 - Medios
| # | Item | Esforco |
|---|------|---------|
| 15 | Usar ErrorType ao inves de string matching | 1h |
| 16 | Remover DTOs nao utilizados | 15 min |
| 17 | Implementar ou remover ReportsController | 2h |
| 18 | Definir precisao decimal para Money | 15 min |
| 19 | Documentar estrategia de Amount positivo vs TransactionType | 30 min |

---

## Resumo Final

| Severidade | Quantidade |
|------------|-----------|
| CRITICO | 7 |
| ALTO | 8 |
| MEDIO | 5 |
| BAIXO | 0 |
| INFO | 1 |
