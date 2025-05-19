using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskItems.Queries.GetById
{
    public class GetTaskItemByIdQueryHandler : IRequestHandler<GetTaskItemByIdQuery, GetTaskItemByIdResult>
    {
        private readonly ITaskItemService _taskItemService;

        public GetTaskItemByIdQueryHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<GetTaskItemByIdResult> Handle(GetTaskItemByIdQuery query, CancellationToken cancellationToken)
        {
            TaskItemDTO? result = await _taskItemService.GetByIdAsync(query.Id, cancellationToken);
            GetTaskItemByIdResult response = new(result);
            return response;
        }
    }
}
