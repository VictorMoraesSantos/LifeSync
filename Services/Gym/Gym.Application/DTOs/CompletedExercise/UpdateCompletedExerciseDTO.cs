using Gym.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Application.DTOs.CompletedExercise
{
    public record UpdateCompletedExerciseDTO(
        int Id,
        SetCount SetsCompleted,
        RepetitionCount RepetitionsCompleted,
        Weight? WeightUsed,
        string? Notes);
}
