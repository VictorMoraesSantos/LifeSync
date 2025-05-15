using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskItems.Queries.GetByPriority
{
    public class GetByPriorityQueryHandler : IRequestHandler<GetByPriorityQuery, GetByPriorityResult>
    {
        private readonly ITaskItemService _taskItemService;

        public GetByPriorityQueryHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<GetByPriorityResult> Handle(GetByPriorityQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
                throw new BadRequestException(nameof(query), "Query cannot be null");

            IEnumerable<TaskItemDTO> result = await _taskItemService.GetTaskItemsByPriorityAsync(query.UserId, query.Priority, cancellationToken);
            GetByPriorityResult response = new(result);
            return response;
        }
    }
}
