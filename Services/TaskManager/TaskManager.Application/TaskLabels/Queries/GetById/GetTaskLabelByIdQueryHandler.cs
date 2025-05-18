using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskLabels.Queries.GetById
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
            if(query == null)
                throw new BadRequestException("Query cannot be null");

            TaskLabelDTO result = await _taskLabelService.GetTaskLabelByIdAsync(query.Id, cancellationToken);
            GetTaskLabelByIdResult response = new(result);
            return response;
        }
    }
}
