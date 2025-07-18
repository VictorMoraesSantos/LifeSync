using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.Routine.Queries.GetRoutineById
{
    public class GetRoutineByIdQueryHandler : IQueryHandler<GetRoutineByIdQuery, GetRoutineByIdResponse>
    {
        private readonly IRoutineService _routineService;

        public GetRoutineByIdQueryHandler(IRoutineService routineService)
        {
            _routineService = routineService;
        }

        public async Task<Result<GetRoutineByIdResponse>> Handle(GetRoutineByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _routineService.GetByIdAsync(query.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result<GetRoutineByIdResponse>.Failure(result.Error!);

            return Result<GetRoutineByIdResponse>.Success(new GetRoutineByIdResponse(result.Value!));
        }
    }
}
