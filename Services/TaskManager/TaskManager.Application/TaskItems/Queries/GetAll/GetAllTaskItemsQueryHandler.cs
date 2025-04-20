using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Mapping;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.TaskItems.Queries.GetAll
{
    public class GetAllTaskItemsQueryHandler : IRequestHandler<GetAllTaskItemsQuery, IEnumerable<TaskItemDTO>>
    {
        private readonly ITaskItemRepository _taskItemRepository;

        public GetAllTaskItemsQueryHandler(ITaskItemRepository taskItemRepository)
        {
            _taskItemRepository = taskItemRepository;
        }

        public async Task<IEnumerable<TaskItemDTO>> Handle(GetAllTaskItemsQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
                throw new BadRequestException(nameof(query), "Query cannot be null");

            IEnumerable<TaskItem?> taskItems = await _taskItemRepository.GetAll(cancellationToken);
            if (taskItems == null || !taskItems.Any())
                throw new NotFoundException("TaskItems was not found");

            IEnumerable<TaskItemDTO> taskItemDTOs = taskItems.Select(TaskItemMapper.ToDTO);
            return taskItemDTOs;
        }
    }
}
