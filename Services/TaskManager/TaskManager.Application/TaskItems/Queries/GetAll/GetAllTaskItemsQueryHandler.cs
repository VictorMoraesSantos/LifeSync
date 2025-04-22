using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskItems.Queries.GetAll
{
    public class GetAllTaskItemsQueryHandler : IRequestHandler<GetAllTaskItemsQuery, GetAllTaskItemsResponse>
    {
        private readonly ITaskItemService _taskItemService;

        public GetAllTaskItemsQueryHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<GetAllTaskItemsResponse> Handle(GetAllTaskItemsQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
                throw new BadRequestException(nameof(query), "Query cannot be null");

            IEnumerable<TaskItemDTO?> result = await _taskItemService.GetAllTaskItemsAsync(cancellationToken);
            GetAllTaskItemsResponse response = new(result);
            return response;
        }
    }
}
