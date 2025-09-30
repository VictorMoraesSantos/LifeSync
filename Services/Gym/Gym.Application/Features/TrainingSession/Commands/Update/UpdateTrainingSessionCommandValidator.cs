using FluentValidation;

namespace Gym.Application.Features.TrainingSession.Commands.UpdateTrainingSession
{
    public class UpdateTrainingSessionCommandValidator : AbstractValidator<UpdateTrainingSessionCommand>
    {
        public UpdateTrainingSessionCommandValidator()
        {
            RuleFor(command => command.Id)
                .GreaterThan(0).WithMessage("O identificador da sessão de treino deve ser maior que zero.");

            RuleFor(command => command.RoutineId)
                .GreaterThan(0).WithMessage("O identificador da rotina deve ser maior que zero.");

            RuleFor(command => command.StartTime)
                .NotEmpty().WithMessage("A hora de início é obrigatória.")
                .LessThan(command => command.EndTime).WithMessage("A hora de início deve ser anterior à hora de término.");

            RuleFor(command => command.EndTime)
                .NotEmpty().WithMessage("A hora de término é obrigatória.")
                .GreaterThan(command => command.StartTime).WithMessage("A hora de término deve ser posterior à hora de início.");

            RuleFor(command => command.Notes)
                .MaximumLength(1000).WithMessage("As anotações devem ter no máximo 1000 caracteres.")
                .When(command => !string.IsNullOrEmpty(command.Notes));
        }
    }
}