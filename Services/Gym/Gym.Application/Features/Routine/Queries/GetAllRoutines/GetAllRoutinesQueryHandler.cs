using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;
using System.Runtime.InteropServices;

namespace Gym.Application.Features.Routine.Queries.GetAllRoutines
{
    public class GetAllRoutinesQueryHandler : IQueryHandler<GetAllRoutinesQuery, GetAllRoutinesResponse>
    {
        private readonly IRoutineService _routineService;

        public GetAllRoutinesQueryHandler(IRoutineService routineService)
        {
            _routineService = routineService;
        }

        public async Task<Result<GetAllRoutinesResponse>> Handle(GetAllRoutinesQuery query, CancellationToken cancellationToken)
        {
            var result = await _routineService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess)
                return Result<GetAllRoutinesResponse>.Failure(result.Error!);

            return Result<GetAllRoutinesResponse>.Success(new GetAllRoutinesResponse(result.Value!));
        }
    }
}
