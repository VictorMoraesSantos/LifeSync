using BuildingBlocks.CQRS.Queries;
using Gym.Application.DTOs.Routine;

namespace Gym.Application.Features.Routine.Queries.GetById
{
    public record GetRoutineByIdQuery(int Id) : IQuery<GetRoutineByIdResult>;
    public record GetRoutineByIdResult(RoutineDTO Routine);
}
