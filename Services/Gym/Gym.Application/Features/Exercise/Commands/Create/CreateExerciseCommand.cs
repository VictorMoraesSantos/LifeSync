using BuildingBlocks.CQRS.Commands;
using Gym.Domain.Enums;

namespace Gym.Application.Features.Exercise.Commands.CreateExercise
{
    public record CreateExerciseCommand(
        string Name,
        string Description,
        MuscleGroup MuscleGroup,
        ExerciseType ExerciseType,
        EquipmentType? EquipmentType)
        : ICommand<CreateExerciseResult>;

    public record CreateExerciseResult(int Id);
}
