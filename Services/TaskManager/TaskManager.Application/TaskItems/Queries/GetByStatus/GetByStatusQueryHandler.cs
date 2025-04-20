using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Mapping;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.TaskItems.Queries.GetByStatus
{
    public class GetByStatusQueryHandler : IRequestHandler<GetByStatusQuery, IEnumerable<TaskItemDTO>>
    {
        private readonly ITaskItemRepository _taskItemRepository;

        public GetByStatusQueryHandler(ITaskItemRepository taskItemRepository)
        {
            _taskItemRepository = taskItemRepository;
        }

        public async Task<IEnumerable<TaskItemDTO>> Handle(GetByStatusQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
                throw new BadRequestException(nameof(query), "Query cannot be null");

            IEnumerable<TaskItem?> taskItems = await _taskItemRepository.GetByStatus(query.UserId, query.Status, cancellationToken);
            if (taskItems == null || !taskItems.Any())
                throw new NotFoundException("TaskItems was not found");

            IEnumerable<TaskItemDTO> taskItemDTOs = taskItems.Select(TaskItemMapper.ToDTO);

            return taskItemDTOs;
        }
    }
}
