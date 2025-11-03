using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskLabels.Queries.GetByFilter
{
    public class GetTaskLabelsByFilterQueryHandler : IQueryHandler<GetTaskLabelsByFilterQuery, GetTaskLabelsByFilterResult>
    {
        private readonly ITaskLabelService _taskLabelService;

        public GetTaskLabelsByFilterQueryHandler(ITaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;
        }

        public async Task<Result<GetTaskLabelsByFilterResult>> Handle(GetTaskLabelsByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _taskLabelService.GetByFilterAsync(query.filter, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetTaskLabelsByFilterResult>(result.Error!);

            return Result.Success(new GetTaskLabelsByFilterResult(result.Value.Items, result.Value.Pagination));
        }
    }
}
