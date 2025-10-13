using FluentValidation;

namespace Nutrition.Application.Features.DailyProgress.Commands.Update
{
    public class UpdateDailyProgressCommandValidator : AbstractValidator<UpdateDailyProgressCommand>
    {
        public UpdateDailyProgressCommandValidator()
        {
            RuleFor(command => command.CaloriesConsumed)
                .NotEmpty().WithMessage("As calorias consumidas são obrigatórias.");

            RuleFor(command => command.CaloriesConsumed)
                .GreaterThanOrEqualTo(0).WithMessage("As calorias consumidas não podem ser negativas.");

            RuleFor(command => command.LiquidsConsumedMl)
                .GreaterThanOrEqualTo(0).WithMessage("A quantidade de líquidos consumidos não pode ser negativa.");
        }
    }
}