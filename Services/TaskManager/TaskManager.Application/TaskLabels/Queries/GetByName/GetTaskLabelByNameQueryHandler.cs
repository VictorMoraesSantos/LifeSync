using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskLabels.Queries.GetByName
{
    public class GetTaskLabelByNameQueryHandler : IRequestHandler<GetTaskLabelByNameQuery, GetTaskLabelByNameResult>
    {
        private readonly ITaskLabelService _taskLabelService;

        public GetTaskLabelByNameQueryHandler(ITaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;
        }

        public async Task<GetTaskLabelByNameResult> Handle(GetTaskLabelByNameQuery query, CancellationToken cancellationToken)
        {
            if(query == null)
                throw new BadRequestException("Query cannot be null");

            IEnumerable<TaskLabelDTO>? result = await _taskLabelService.GetTaskLabelsByNameAsync(query.UserId, query.Name, cancellationToken);
            GetTaskLabelByNameResult response = new GetTaskLabelByNameResult(result);
            return response;
        }
    }
}
