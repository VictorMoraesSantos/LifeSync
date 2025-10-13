using BuildingBlocks.CQRS.Queries;
using Gym.Application.DTOs.RoutineExercise;

namespace Gym.Application.Features.RoutineExercise.Queries.GetAll
{
    public record GetAllRoutineExercisesQuery() : IQuery<GetAllRoutineExercisesResult>;
    public record GetAllRoutineExercisesResult(IEnumerable<RoutineExerciseDTO> RoutineExercise);
}
