using FluentValidation;

namespace Gym.Application.Features.CompletedExercise.Commands.CreateCompletedExercise
{
    public class CreateCompletedExerciseCommandValidator : AbstractValidator<CreateCompletedExerciseCommand>
    {
        public CreateCompletedExerciseCommandValidator()
        {
            RuleFor(command => command.TrainingSessionId)
                .GreaterThan(0).WithMessage("O identificador da sessão de treino deve ser maior que zero.");

            RuleFor(command => command.ExerciseId)
                .GreaterThan(0).WithMessage("O identificador do exercício deve ser maior que zero.");

            RuleFor(command => command.RoutineExerciseId)
                .GreaterThan(0).WithMessage("O identificador do exercício na rotina deve ser maior que zero.");

            RuleFor(command => command.SetsCompleted)
                .NotNull().WithMessage("O número de séries completadas é obrigatório.")
                .Must(sets => sets.Value > 0).WithMessage("O número de séries completadas deve ser maior que zero.");

            RuleFor(command => command.RepetitionsCompleted)
                .NotNull().WithMessage("O número de repetições completadas é obrigatório.")
                .Must(reps => reps.Value > 0).WithMessage("O número de repetições completadas deve ser maior que zero.");

            When(command => command.WeightUsed != null, () =>
            {
                RuleFor(command => command.WeightUsed).Must(weight => weight.Value >= 0).WithMessage("O peso utilizado não pode ser negativo.");
            });

            RuleFor(command => command.Notes)
                .MaximumLength(500).WithMessage("As anotações devem ter no máximo 500 caracteres.")
                .When(command => !string.IsNullOrEmpty(command.Notes));
        }
    }
}