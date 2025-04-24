using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskLabels.Queries.GetAll
{
    public class GetAllTaskLabelsQueryHandler : IRequestHandler<GetAllTaskLabelsQuery, GetAllTaskLabelsResponse>
    {
        private readonly ITaskLabelService _taskLabelService;

        public GetAllTaskLabelsQueryHandler(ITaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;
        }

        public async Task<GetAllTaskLabelsResponse> Handle(GetAllTaskLabelsQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<TaskLabelDTO> result = await _taskLabelService.GetAllTaskLabelsAsync(cancellationToken);
            GetAllTaskLabelsResponse response = new(result);
            return response;
        }
    }
}
