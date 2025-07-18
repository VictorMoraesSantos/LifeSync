using BuildingBlocks.CQRS.Queries;
using Gym.Application.DTOs.Exercise;

namespace Gym.Application.Features.Exercise.Queries.GetById
{
    public record GetExerciseByIdQuery(int Id) : IQuery<GetExerciseByIdResult>; 
    public record GetExerciseByIdResult(ExerciseDTO Exercise);
}
