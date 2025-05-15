using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskItems.Queries.GetAll
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
            IEnumerable<TaskItemDTO?> result = await _taskItemService.GetAllTaskItemsAsync(cancellationToken);
            GetAllTaskItemsResult response = new(result);
            return response;
        }
    }
}
