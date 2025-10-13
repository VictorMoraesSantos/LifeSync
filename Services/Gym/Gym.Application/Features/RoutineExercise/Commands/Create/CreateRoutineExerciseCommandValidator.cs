using FluentValidation;

namespace Gym.Application.Features.RoutineExercise.Commands.Create
{
    public class CreateRoutineExerciseCommandValidator : AbstractValidator<CreateRoutineExerciseCommand>
    {
        public CreateRoutineExerciseCommandValidator()
        {
            RuleFor(command => command.RoutineId)
                .GreaterThan(0).WithMessage("O identificador da rotina deve ser maior que zero.");

            RuleFor(command => command.ExerciseId)
                .GreaterThan(0).WithMessage("O identificador do exercício deve ser maior que zero.");

            RuleFor(command => command.Sets)
                .NotNull().WithMessage("O número de séries é obrigatório.")
                .Must(sets => sets.Value > 0).WithMessage("O número de séries deve ser maior que zero.");

            RuleFor(command => command.Repetitions)
                .NotNull().WithMessage("O número de repetições é obrigatório.")
                .Must(reps => reps.Value > 0).WithMessage("O número de repetições deve ser maior que zero.");

            RuleFor(command => command.RestBetweenSets)
                .NotNull().WithMessage("O tempo de descanso entre séries é obrigatório.")
                .Must(rest => rest.Value > 0).WithMessage("O tempo de descanso entre séries deve ser maior que zero segundos.");

            When(command => command.RecommendedWeight != null, () =>
            {
                RuleFor(command => command.RecommendedWeight)
                .Must(weight => weight.Value >= 0).WithMessage("O peso recomendado não pode ser negativo.");
            });

            RuleFor(command => command.Instructions)
                .MaximumLength(500).WithMessage("As instruções devem ter no máximo 500 caracteres.")
                .When(command => !string.IsNullOrEmpty(command.Instructions));
        }
    }
}