using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskItems.Queries.GetByTitle
{
    public class GetByTitleQueryHandler : IRequestHandler<GetByTitleQuery, GetByTitleResponse>
    {
        private readonly ITaskItemService _taskItemService;

        public GetByTitleQueryHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<GetByTitleResponse> Handle(GetByTitleQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
                throw new BadRequestException(nameof(query), "Query cannot be null");

            IEnumerable<TaskItemDTO> result = await _taskItemService.GetTaskItemsTitleAsync(query.UserId, query.Title, cancellationToken);
            GetByTitleResponse response = new(result);
            return response;
        }
    }
}
