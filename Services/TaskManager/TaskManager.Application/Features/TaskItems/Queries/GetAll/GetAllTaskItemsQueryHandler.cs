using BuildingBlocks.CQRS.Request;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Queries.GetAll
{
    public class GetAllTaskItemsQueryHandler : IRequestHandler<GetAllTaskItemsQuery, GetAllTaskItemsResult>
    {
        private readonly ITaskItemService _taskItemService;

        public GetAllTaskItemsQueryHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<GetAllTaskItemsResult> Handle(GetAllTaskItemsQuery query, CancellationToken cancellationToken)
        {
            var result = await _taskItemService.GetAllAsync(cancellationToken);
            return new GetAllTaskItemsResult(result);
        }
    }
}
