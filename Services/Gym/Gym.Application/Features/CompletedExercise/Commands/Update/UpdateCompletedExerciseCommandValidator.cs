using FluentValidation;
using Gym.Application.Features.CompletedExercise.Commands.UpdateCompletedExercise;

namespace Gym.Application.Features.CompletedExercise.Commands.Update
{
    public class UpdateCompletedExerciseCommandValidator : AbstractValidator<UpdateCompletedExerciseCommand>
    {
        public UpdateCompletedExerciseCommandValidator()
        {
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
