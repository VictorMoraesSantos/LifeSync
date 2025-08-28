using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.Routine.Queries.GetAllRoutines
{
    public class GetAllRoutinesQueryHandler : IQueryHandler<GetAllRoutinesQuery, GetAllRoutinesResult>
    {
        private readonly IRoutineService _routineService;

        public GetAllRoutinesQueryHandler(IRoutineService routineService)
        {
            _routineService = routineService;
        }

        public async Task<Result<GetAllRoutinesResult>> Handle(GetAllRoutinesQuery query, CancellationToken cancellationToken)
        {
            var result = await _routineService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess)
                return Result<GetAllRoutinesResult>.Failure(result.Error!);

            return Result<GetAllRoutinesResult>.Success(new GetAllRoutinesResult(result.Value!));
        }
    }
}
