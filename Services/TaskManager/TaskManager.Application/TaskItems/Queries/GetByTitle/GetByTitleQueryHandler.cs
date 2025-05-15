using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskItems.Queries.GetByTitle
{
    public class GetByTitleQueryHandler : IRequestHandler<GetByTitleQuery, GetByTitleResult>
    {
        private readonly ITaskItemService _taskItemService;

        public GetByTitleQueryHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<GetByTitleResult> Handle(GetByTitleQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
                throw new BadRequestException(nameof(query), "Query cannot be null");

            IEnumerable<TaskItemDTO> result = await _taskItemService.GetTaskItemsTitleAsync(query.UserId, query.Title, cancellationToken);
            GetByTitleResult response = new(result);
            return response;
        }
    }
}
