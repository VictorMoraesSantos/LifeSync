using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskLabels.Queries.GetByUserId
{
    public class GetTaskLabelByUserIdQueryHandler : IRequestHandler<GetTaskLabelByUserIdQuery, GetTaskLabelByUserIdResult>
    {
        private readonly ITaskLabelService _taskLabelService;
        public GetTaskLabelByUserIdQueryHandler(ITaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;
        }

        public async Task<GetTaskLabelByUserIdResult> Handle(GetTaskLabelByUserIdQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new BadRequestException("Query cannot be null");

            IEnumerable<TaskLabelDTO>? result = await _taskLabelService.GetTaskLabelsByUserIdAsync(request.UserId, cancellationToken);
            GetTaskLabelByUserIdResult response = new GetTaskLabelByUserIdResult(result);
            return response;
        }
    }
}
