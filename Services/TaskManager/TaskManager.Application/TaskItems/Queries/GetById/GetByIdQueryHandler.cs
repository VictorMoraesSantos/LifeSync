using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskItems.Queries.GetById
{
    public class GetByIdQueryHandler : IRequestHandler<GetTaskItemByIdQuery, GetTaskItemByIdResult>
    {
        private readonly ITaskItemService _taskItemService;

        public GetByIdQueryHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<GetTaskItemByIdResult> Handle(GetTaskItemByIdQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
                throw new BadRequestException("Query cannot be null");

            TaskItemDTO? result = await _taskItemService.GetByIdAsync(query.Id, cancellationToken);
            GetTaskItemByIdResult response = new(result);
            return response;
        }
    }
}
