using BuildingBlocks.CQRS.Commands;
using Gym.Domain.Enums;

namespace Gym.Application.Features.Exercise.Commands.UpdateExercise
{
    public record UpdateExerciseCommand(
        int Id,
        string Name,
        string Description,
        MuscleGroup MuscleGroup,
        ExerciseType ExerciseType,
        EquipmentType? EquipmentType)
        : ICommand<UpdateExerciseResult>;

    public record UpdateExerciseResult(bool IsSuccess);
}
