using MediatR;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskLabels.Queries.GetByFilter
{
    public class GetByFilterQueryHandler : IRequestHandler<GetByFilterQuery, GetByFilterResult>
    {
        private readonly ITaskLabelService _taskLabelService;

        public GetByFilterQueryHandler(ITaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;
        }

        public async Task<GetByFilterResult> Handle(GetByFilterQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<TaskLabelDTO?> result = await _taskLabelService.GetByFilterAsync(query.filter, cancellationToken);
            GetByFilterResult response = new(result);
            return response;
        }
    }
}
