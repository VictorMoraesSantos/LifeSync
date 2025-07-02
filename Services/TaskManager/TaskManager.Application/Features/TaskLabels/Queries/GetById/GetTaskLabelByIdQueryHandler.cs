using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskLabels.Queries.GetById
{
    public class GetTaskLabelByIdQueryHandler : IQueryHandler<GetTaskLabelByIdQuery, GetTaskLabelByIdResult>
    {
        private readonly ITaskLabelService _taskLabelService;

        public GetTaskLabelByIdQueryHandler(ITaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;
        }

        public async Task<Result<GetTaskLabelByIdResult>> Handle(GetTaskLabelByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _taskLabelService.GetByIdAsync(query.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetTaskLabelByIdResult>(result.Error!);

            return Result.Success(new GetTaskLabelByIdResult(result.Value!));
        }
    }
}
