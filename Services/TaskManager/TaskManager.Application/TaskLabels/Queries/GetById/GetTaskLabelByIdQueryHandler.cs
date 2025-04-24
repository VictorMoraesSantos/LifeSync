using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskLabels.Queries.GetById
{
    public class GetTaskLabelByIdQueryHandler : IRequestHandler<GetTaskLabelByIdQuery, GetTaskLabelByIdResponse>
    {
        private readonly ITaskLabelService _taskLabelService;

        public GetTaskLabelByIdQueryHandler(ITaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;
        }

        public async Task<GetTaskLabelByIdResponse> Handle(GetTaskLabelByIdQuery query, CancellationToken cancellationToken)
        {
            if(query == null)
                throw new BadRequestException("Query cannot be null");

            TaskLabelDTO result = await _taskLabelService.GetTaskLabelByIdAsync(query.Id, cancellationToken);
            GetTaskLabelByIdResponse response = new(result);
            return response;
        }
    }
}
