using BuildingBlocks.CQRS.Queries;
using Gym.Application.DTOs.RoutineExercise;

namespace Gym.Application.Features.RoutineExercise.Queries.GetAllExercises
{
    public record GetAllRoutineExercisesQuery() : IQuery<GetAllRoutineExercisesResponse>;
    public record GetAllRoutineExercisesResponse(IEnumerable<RoutineExerciseDTO> RoutineExercise);
}
