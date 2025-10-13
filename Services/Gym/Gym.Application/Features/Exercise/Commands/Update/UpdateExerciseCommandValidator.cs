using FluentValidation;

namespace Gym.Application.Features.Exercise.Commands.Update
{
    public class UpdateExerciseCommandValidator : AbstractValidator<UpdateExerciseCommand>
    {
        public UpdateExerciseCommandValidator()
        {
            RuleFor(command => command.Name)
                .NotEmpty().WithMessage("O nome do exercício é obrigatório.")
                .MaximumLength(100).WithMessage("O nome do exercício deve ter no máximo 100 caracteres.");

            RuleFor(command => command.Description)
                .NotEmpty().WithMessage("A descrição do exercício é obrigatória.")
                .MaximumLength(500).WithMessage("A descrição do exercício deve ter no máximo 500 caracteres.");

            RuleFor(command => command.MuscleGroup)
                .IsInEnum().WithMessage("O grupo muscular informado é inválido.");

            RuleFor(command => command.ExerciseType)
                .IsInEnum().WithMessage("O tipo de exercício informado é inválido.");

            When(command => command.EquipmentType.HasValue, () =>
            {
                RuleFor(command => command.EquipmentType.Value).IsInEnum().WithMessage("O tipo de equipamento informado é inválido.");
            });
        }
    }
}
