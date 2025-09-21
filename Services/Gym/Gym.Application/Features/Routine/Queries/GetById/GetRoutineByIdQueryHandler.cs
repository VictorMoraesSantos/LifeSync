using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.Routine.Queries.GetRoutineById
{
    public class GetRoutineByIdQueryHandler : IQueryHandler<GetRoutineByIdQuery, GetRoutineByIdResult>
    {
        private readonly IRoutineService _routineService;

        public GetRoutineByIdQueryHandler(IRoutineService routineService)
        {
            _routineService = routineService;
        }

        public async Task<Result<GetRoutineByIdResult>> Handle(GetRoutineByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _routineService.GetByIdAsync(query.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result<GetRoutineByIdResult>.Failure(result.Error!);

            return Result<GetRoutineByIdResult>.Success(new GetRoutineByIdResult(result.Value!));
        }
    }
}
