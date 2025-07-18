using BuildingBlocks.CQRS.Queries;
using Gym.Application.DTOs.Routine;

namespace Gym.Application.Features.Routine.Queries.GetAllRoutines
{
    public record GetAllRoutinesQuery() : IQuery<GetAllRoutinesResponse>;
    public record GetAllRoutinesResponse(IEnumerable<RoutineDTO> Routines);
}
