using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskLabels.Queries.GetByUserId
{
    public class GetTaskLabelByUserIdQueryHandler : IRequestHandler<GetTaskLabelByUserIdQuery, GetTaskLabelByUserIdResponse>
    {
        private readonly ITaskLabelService _taskLabelService;
        public GetTaskLabelByUserIdQueryHandler(ITaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;
        }

        public async Task<GetTaskLabelByUserIdResponse> Handle(GetTaskLabelByUserIdQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new BadRequestException("Query cannot be null");

            IEnumerable<TaskLabelDTO>? result = await _taskLabelService.GetTaskLabelsByUserIdAsync(request.UserId, cancellationToken);
            GetTaskLabelByUserIdResponse response = new GetTaskLabelByUserIdResponse(result);
            return response;
        }
    }
}
