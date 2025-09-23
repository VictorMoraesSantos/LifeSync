using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Queries.GetByUser
{
    public class GetByUserQueryHandler : IQueryHandler<GetByUserQuery, GetByUserResponse>
    {
        private readonly ITaskItemService _taskItemService;

        public GetByUserQueryHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<Result<GetByUserResponse>> Handle(GetByUserQuery query, CancellationToken cancellationToken)
        {
            var result = await _taskItemService.FindAsync(t => t.UserId == query.UserId, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetByUserResponse>(result.Error!);

            return Result.Success(new GetByUserResponse(result.Value!));
        }
    }
}
