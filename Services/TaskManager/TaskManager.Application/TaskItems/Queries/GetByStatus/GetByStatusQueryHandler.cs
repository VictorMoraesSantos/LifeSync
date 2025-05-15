using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskItems.Queries.GetByStatus
{
    public class GetByStatusQueryHandler : IRequestHandler<GetByStatusQuery, GetByStatusResult>
    {
        private readonly ITaskItemService _taskItemService;

        public GetByStatusQueryHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<GetByStatusResult> Handle(GetByStatusQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
                throw new BadRequestException(nameof(query), "Query cannot be null");

            IEnumerable<TaskItemDTO> result = await _taskItemService.GetTaskItemsByStatusAsync(query.UserId, query.Status, cancellationToken);
            GetByStatusResult response = new(result);
            return response;
        }
    }
}
