using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskLabels.Queries.GetAll
{
    public class GetAllTaskLabelsQueryHandler : IQueryHandler<GetAllTaskLabelsQuery, GetAllTaskLabelsResult>
    {
        private readonly ITaskLabelService _taskLabelService;

        public GetAllTaskLabelsQueryHandler(ITaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;
        }

        public async Task<Result<GetAllTaskLabelsResult>> Handle(GetAllTaskLabelsQuery query, CancellationToken cancellationToken)
        {
            var result = await _taskLabelService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetAllTaskLabelsResult>(result.Error!);

            return Result.Success(new GetAllTaskLabelsResult(result.Value!));
        }
    }
}
