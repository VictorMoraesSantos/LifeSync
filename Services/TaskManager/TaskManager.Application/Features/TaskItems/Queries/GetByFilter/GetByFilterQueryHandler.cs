using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Queries.GetByFilter
{
    public class GetByFilterQueryHandler : IQueryHandler<GetByFilterQuery, GetByFilterResult>
    {
        private readonly ITaskItemService _taskItemService;

        public GetByFilterQueryHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<Result<GetByFilterResult>> Handle(GetByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _taskItemService.GetByFilterAsync(query.Filter, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetByFilterResult>(result.Error!);

            return Result.Success(new GetByFilterResult(result.Value!));
        }
    }
}
