using Core.Application.DTO;
using Gym.Domain.ValueObjects;

namespace Gym.Application.DTOs.RoutineExercise
{
    public record RoutineExerciseDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        int RoutineId,
        int ExerciseId,
        SetCount Sets,
        RepetitionCount Repetitions,
        RestTime RestBetweenSets,
        Weight? RecommendedWeight,
        string? Instructions)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
