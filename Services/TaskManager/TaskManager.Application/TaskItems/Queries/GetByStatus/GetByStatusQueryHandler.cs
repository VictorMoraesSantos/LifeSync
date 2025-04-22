using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskItems.Queries.GetByStatus
{
    public class GetByStatusQueryHandler : IRequestHandler<GetByStatusQuery, GetByStatusResponse>
    {
        private readonly ITaskItemService _taskItemService;

        public GetByStatusQueryHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<GetByStatusResponse> Handle(GetByStatusQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
                throw new BadRequestException(nameof(query), "Query cannot be null");

            IEnumerable<TaskItemDTO> result = await _taskItemService.GetTaskItemsByStatusAsync(query.UserId, query.Status, cancellationToken);
            GetByStatusResponse response = new(result);
            return response;
        }
    }
}
