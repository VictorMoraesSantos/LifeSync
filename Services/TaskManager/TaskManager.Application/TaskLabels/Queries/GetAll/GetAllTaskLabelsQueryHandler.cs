using MediatR;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskLabels.Queries.GetAll
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
            IEnumerable<TaskLabelDTO> result = await _taskLabelService.GetAllTaskLabelsAsync(cancellationToken);
            GetAllTaskLabelsResult response = new(result);
            return response;
        }
    }
}
