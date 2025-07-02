using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskLabels.Queries.GetByFilter
{
    public class GetByFilterQueryHandler : IQueryHandler<GetByFilterQuery, GetByFilterResult>
    {
        private readonly ITaskLabelService _taskLabelService;

        public GetByFilterQueryHandler(ITaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;
        }

        public async Task<Result<GetByFilterResult>> Handle(GetByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _taskLabelService.GetByFilterAsync(query.filter, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetByFilterResult>(result.Error!);

            return Result.Success(new GetByFilterResult(result.Value!));
        }
    }
}
