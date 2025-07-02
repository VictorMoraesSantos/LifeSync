using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Queries.GetById
{
    public class GetTaskItemByIdQueryHandler : IQueryHandler<GetTaskItemByIdQuery, GetTaskItemByIdResult>
    {
        private readonly ITaskItemService _taskItemService;

        public GetTaskItemByIdQueryHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<Result<GetTaskItemByIdResult>> Handle(GetTaskItemByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _taskItemService.GetByIdAsync(query.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetTaskItemByIdResult>(result.Error!);

            return Result.Success(new GetTaskItemByIdResult(result.Value!));
        }
    }
}
