using Core.Application.DTO;
using Gym.Domain.ValueObjects;

namespace Gym.Application.DTOs.CompletedExercise
{
    public record CompletedExerciseDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        int TrainingSessionId,
        int RoutineExerciseId,
        SetCount SetsCompleted,
        RepetitionCount RepetitionsCompleted,
        Weight? WeightUsed,
        string? Notes)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
