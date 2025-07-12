using Core.Application.DTO;
using Gym.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Application.DTOs.CompletedExercise
{
    public record CompletedExerciseDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        int TrainingSessionId,
        int ExerciseId,
        int RoutineExerciseId,
        SetCount SetsCompleted,
        RepetitionCount RepetitionsCompleted,
        Weight? WeightUsed,
        string? Notes)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
