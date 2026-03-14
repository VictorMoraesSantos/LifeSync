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

            When(command => command.IsRecurring, () =>
            {
                RuleFor(command => command.Frequency)
                    .NotNull().WithMessage("A frequência de recorrência é obrigatória para transações recorrentes.")
                    .IsInEnum().WithMessage("A frequência de recorrência informada é inválida.");

                RuleFor(command => command.RecurrenceEndDate)
                    .GreaterThan(command => command.TransactionDate).WithMessage("A data de término da recorrência deve ser posterior à data da transação.")
                    .When(command => command.RecurrenceEndDate.HasValue);

                RuleFor(command => command.MaxOccurrences)
                    .GreaterThan(0).WithMessage("O número máximo de ocorrências deve ser maior que zero.")
                    .When(command => command.MaxOccurrences.HasValue);
            });
        }
    }
}