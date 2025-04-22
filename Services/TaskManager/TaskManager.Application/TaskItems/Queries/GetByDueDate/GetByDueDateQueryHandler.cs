using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskItems.Queries.GetByDueDate
{
    public class GetByDueDateQueryHandler : IRequestHandler<GetByDueDateQuery, GetByDueDateResponse>
    {
        private readonly ITaskItemService _taskItemService;

        public GetByDueDateQueryHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<GetByDueDateResponse> Handle(GetByDueDateQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
                throw new BadRequestException(nameof(query), "Query cannot be null");

            IEnumerable<TaskItemDTO> result = await _taskItemService.GetTaskItemsByDueDateAsync(query.UserId, query.DueDate, cancellationToken);
            GetByDueDateResponse response = new(result);
            return response;
        }
    }
}
