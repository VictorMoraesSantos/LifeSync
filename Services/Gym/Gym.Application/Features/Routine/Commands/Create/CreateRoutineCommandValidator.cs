using FluentValidation;

namespace Gym.Application.Features.Routine.Commands.CreateRoutine
{
    public class CreateRoutineCommandValidator : AbstractValidator<CreateRoutineCommand>
    {
        public CreateRoutineCommandValidator()
        {
            RuleFor(command => command.Name)
                .NotEmpty().WithMessage("O nome da rotina é obrigatório.")
                .MinimumLength(3).WithMessage("O nome da rotina deve ter no mínimo 3 caracteres.")
                .MaximumLength(100).WithMessage("O nome da rotina deve ter no máximo 100 caracteres.");

            RuleFor(command => command.Description)
                .NotEmpty().WithMessage("A descrição da rotina é obrigatória.")
                .MinimumLength(5).WithMessage("A descrição da rotina deve ter no mínimo 5 caracteres.")
                .MaximumLength(500).WithMessage("A descrição da rotina deve ter no máximo 500 caracteres.");
        }
    }
}