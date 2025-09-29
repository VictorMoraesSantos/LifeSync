using FluentValidation;

namespace Nutrition.Application.Features.DailyProgress.Commands.SetGoal
{
    public class SetGoalCommandValidator : AbstractValidator<SetGoalCommand>
    {
        public SetGoalCommandValidator()
        {
            RuleFor(command => command.Goal)
                .NotNull().WithMessage("A meta diária é obrigatória.");
        }
    }
}
