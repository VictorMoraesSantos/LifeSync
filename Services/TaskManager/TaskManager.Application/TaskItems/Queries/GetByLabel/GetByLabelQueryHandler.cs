using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskItems.Queries.GetByLabel
{
    public class GetByLabelQueryHandler : IRequestHandler<GetByLabelQuery, GetByLabelResult>
    {
        private readonly ITaskItemService _taskItemService;

        public GetByLabelQueryHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<GetByLabelResult> Handle(GetByLabelQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
                throw new BadRequestException(nameof(query), "Query cannot be null");

            IEnumerable<TaskItemDTO> result = await _taskItemService.GetTaskItemsByLabelIdAsync(query.UserId, query.LabelId, cancellationToken);
            GetByLabelResult response = new(result);
            return response;
        }
    }
}
