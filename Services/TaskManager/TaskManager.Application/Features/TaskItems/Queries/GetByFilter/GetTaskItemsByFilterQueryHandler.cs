using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Queries.GetByFilter
{
    public class GetTaskItemsByFilterQueryHandler : IQueryHandler<GetTaskItemsByFilterQuery, GetTaskItemsByFilterResult>
    {
        private readonly ITaskItemService _taskItemService;

        public GetTaskItemsByFilterQueryHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<Result<GetTaskItemsByFilterResult>> Handle(GetTaskItemsByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _taskItemService.GetByFilterAsync(query.Filter, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetTaskItemsByFilterResult>(result.Error!);

            return Result.Success(new GetTaskItemsByFilterResult(result.Value.Items, result.Value.Pagination!));
        }
    }
}
