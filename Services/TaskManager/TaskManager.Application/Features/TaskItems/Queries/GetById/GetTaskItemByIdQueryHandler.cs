using BuildingBlocks.CQRS.Request;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Queries.GetById
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
            var result = await _taskItemService.GetByIdAsync(query.Id, cancellationToken);
            return new GetTaskItemByIdResult(result);
        }
    }
}
