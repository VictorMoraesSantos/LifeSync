using BuildingBlocks.CQRS.Queries;
using Gym.Application.DTOs.CompletedExercise;

namespace Gym.Application.Features.CompletedExercise.Queries.GetCompletedExerciseById
{
    public record GetCompletedExerciseByIdQuery(int Id) : IQuery<CompletedExerciseResult>;
    public record CompletedExerciseResult(CompletedExerciseDTO CompletedExercise);
}
