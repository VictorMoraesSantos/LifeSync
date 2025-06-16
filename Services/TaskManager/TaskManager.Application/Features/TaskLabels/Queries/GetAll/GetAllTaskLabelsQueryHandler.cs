using BuildingBlocks.CQRS.Request;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskLabels.Queries.GetAll
{
    public class GetAllTaskLabelsQueryHandler : IRequestHandler<GetAllTaskLabelsQuery, GetAllTaskLabelsResult>
    {
        private readonly ITaskLabelService _taskLabelService;

        public GetAllTaskLabelsQueryHandler(ITaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;
        }

        public async Task<GetAllTaskLabelsResult> Handle(GetAllTaskLabelsQuery query, CancellationToken cancellationToken)
        {
            var result = await _taskLabelService.GetAllAsync(cancellationToken);
            return new GetAllTaskLabelsResult(result);
        }
    }
}
