using BuildingBlocks.CQRS.Queries;
using Gym.Application.DTOs.RoutineExercise;

namespace Gym.Application.Features.RoutineExercise.Queries.GetById
{
    public record GetRoutineExerciseByIdQuery(int Id) : IQuery<GetRoutineExerciseByIdResult>;
    public record GetRoutineExerciseByIdResult(RoutineExerciseDTO RoutineExercise);
}
