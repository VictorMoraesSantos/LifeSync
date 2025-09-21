using BuildingBlocks.CQRS.Queries;
using Gym.Application.DTOs.CompletedExercise;

namespace Gym.Application.Features.CompletedExercise.Queries.GetAllCompletedExercises
{
    public record GetAllCompletedExercisesQuery() : IQuery<GetAllCompletedExercisesResult>;
    public record GetAllCompletedExercisesResult(IEnumerable<CompletedExerciseDTO> CompletedExercises);
}
