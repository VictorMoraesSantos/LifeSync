using BuildingBlocks.CQRS.Request;
using TaskManager.Application.DTOs.TaskItem;
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
            IEnumerable<TaskItemDTO?> result = await _taskItemService.GetAllAsync(cancellationToken);
            GetAllTaskItemsResult response = new(result);
            return response;
        }
    }
}
