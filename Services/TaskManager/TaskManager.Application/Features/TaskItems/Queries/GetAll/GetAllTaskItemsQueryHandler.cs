using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Queries.GetAll
{
    public class GetAllTaskItemsQueryHandler : IQueryHandler<GetAllTaskItemsQuery, GetAllTaskItemsResult>
    {
        private readonly ITaskItemService _taskItemService;

        public GetAllTaskItemsQueryHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<Result<GetAllTaskItemsResult>> Handle(GetAllTaskItemsQuery query, CancellationToken cancellationToken)
        {
            var result = await _taskItemService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetAllTaskItemsResult>(result.Error!);

            return Result.Success(new GetAllTaskItemsResult(result.Value!));
        }
    }
}
