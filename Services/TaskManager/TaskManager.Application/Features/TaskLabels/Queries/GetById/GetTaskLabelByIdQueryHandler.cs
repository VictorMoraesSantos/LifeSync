using BuildingBlocks.CQRS.Request;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskLabels.Queries.GetById
{
    public class GetTaskLabelByIdQueryHandler : IRequestHandler<GetTaskLabelByIdQuery, GetTaskLabelByIdResult>
    {
        private readonly ITaskLabelService _taskLabelService;

        public GetTaskLabelByIdQueryHandler(ITaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;
        }

        public async Task<GetTaskLabelByIdResult> Handle(GetTaskLabelByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _taskLabelService.GetByIdAsync(query.Id, cancellationToken);
            return new GetTaskLabelByIdResult(result);
        }
    }
}
