using BuildingBlocks.CQRS.Queries;
using Gym.Application.DTOs.Routine;

namespace Gym.Application.Features.Routine.Queries.GetAllRoutines
{
    public record GetAllRoutinesQuery() : IQuery<GetAllRoutinesResult>;
    public record GetAllRoutinesResult(IEnumerable<RoutineDTO> Routines);
}
