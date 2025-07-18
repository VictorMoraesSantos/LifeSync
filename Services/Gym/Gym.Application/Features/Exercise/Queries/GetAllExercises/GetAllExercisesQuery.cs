using BuildingBlocks.CQRS.Queries;
using Gym.Application.DTOs.Exercise;

namespace Gym.Application.Features.Exercise.Queries.GetAll
{
    public record GetAllExercisesQuery() : IQuery<GetAllExercisesResult>;
    public record GetAllExercisesResult(IEnumerable<ExerciseDTO> Exercises);
}
