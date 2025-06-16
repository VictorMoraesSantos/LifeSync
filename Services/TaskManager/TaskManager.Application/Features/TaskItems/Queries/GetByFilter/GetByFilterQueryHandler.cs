using BuildingBlocks.CQRS.Request;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Queries.GetByFilter
{
    public class GetByFilterQueryHandler : IRequestHandler<GetByFilterQuery, GetByFilterResult>
    {
        private readonly ITaskItemService _taskItemService;

        public GetByFilterQueryHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<GetByFilterResult> Handle(GetByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _taskItemService.GetByFilterAsync(query.Filter, cancellationToken);
            return new GetByFilterResult(result);
        }
    }
}
