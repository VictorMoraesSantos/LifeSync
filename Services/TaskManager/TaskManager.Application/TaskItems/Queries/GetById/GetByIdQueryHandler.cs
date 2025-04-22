using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskItems.Queries.GetById
{
    public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, GetByIdResponse>
    {
        private readonly ITaskItemService _taskItemService;

        public GetByIdQueryHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<GetByIdResponse> Handle(GetByIdQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
                throw new BadRequestException("Query cannot be null");

            TaskItemDTO result = await _taskItemService.GetTaskItemByIdAsync(query.Id, cancellationToken);
            GetByIdResponse response = new(result);
            return response;
        }
    }
}
