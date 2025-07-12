using Gym.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Application.DTOs.CompletedExercise
{
    public record CreateCompletedExerciseDTO(
        int TrainingSessionId,
        int ExerciseId,
        int RoutineExerciseId,
        SetCount SetsCompleted,
        RepetitionCount RepetitionsCompleted,
        Weight? WeightUsed,
        string? Notes);
}
