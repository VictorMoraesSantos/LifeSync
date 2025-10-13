using FluentValidation;

namespace Gym.Application.Features.TrainingSession.Commands.Create
{
    public class CreateTrainingSessionCommandValidator : AbstractValidator<CreateTrainingSessionCommand>
    {
        public CreateTrainingSessionCommandValidator()
        {
            RuleFor(command => command.RoutineId)
                .GreaterThan(0).WithMessage("O identificador da rotina deve ser maior que zero.");

            RuleFor(command => command.StartTime)
                .NotEmpty().WithMessage("A hora de início é obrigatória.")
                .LessThan(command => command.EndTime).WithMessage("A hora de início deve ser anterior à hora de término.");

            RuleFor(command => command.EndTime)
                .NotEmpty().WithMessage("A hora de término é obrigatória.")
                .GreaterThan(command => command.StartTime).WithMessage("A hora de término deve ser posterior à hora de início.");
        }
    }
}