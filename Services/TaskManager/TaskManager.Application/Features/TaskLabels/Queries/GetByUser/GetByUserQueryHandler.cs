using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskLabels.Queries.GetByUser
{
    public class GetByUserQueryHandler : IQueryHandler<GetByUserQuery, GetByUserResponse>
    {
        private readonly ITaskLabelService _taskLabelService;

        public GetByUserQueryHandler(ITaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;
        }

        public async Task<Result<GetByUserResponse>> Handle(GetByUserQuery query, CancellationToken cancellationToken)
        {
            var result = await _taskLabelService.FindAsync(tl => tl.UserId == query.UserId, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetByUserResponse>(result.Error!);

            return Result.Success(new GetByUserResponse(result.Value!));
        }
    }
}
