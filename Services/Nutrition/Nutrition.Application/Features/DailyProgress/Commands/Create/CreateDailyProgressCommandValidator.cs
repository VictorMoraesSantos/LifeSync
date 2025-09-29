using FluentValidation;

namespace Nutrition.Application.Features.DailyProgress.Commands.Create
{
    public class CreateDailyProgressCommandValidator : AbstractValidator<CreateDailyProgressCommand>
    {
        public CreateDailyProgressCommandValidator()
        {
            RuleFor(command => command.Date)
                .NotEmpty().WithMessage("A data é obrigatória.");

            RuleFor(command => command.CaloriesConsumed)
                .GreaterThanOrEqualTo(0).WithMessage("As calorias consumidas não podem ser negativas.");

            RuleFor(command => command.LiquidsConsumedMl)
                .GreaterThanOrEqualTo(0).WithMessage("A quantidade de líquidos consumidos não pode ser negativa.");
        }
    }
}