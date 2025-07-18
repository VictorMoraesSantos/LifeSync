using BuildingBlocks.CQRS.Queries;
using Gym.Application.DTOs.Routine;

namespace Gym.Application.Features.Routine.Queries.GetRoutineById
{
    public record GetRoutineByIdQuery(int Id) : IQuery<GetRoutineByIdResponse>;
    public record GetRoutineByIdResponse(RoutineDTO Routine);
}
