using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Mapping;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.TaskItems.Queries.GetByTitle
{
    public class GetByTitleQueryHandler : IRequestHandler<GetByTitleQuery, IEnumerable<TaskItemDTO>>
    {
        private readonly ITaskItemRepository _taskItemRepository;

        public GetByTitleQueryHandler(ITaskItemRepository taskItemRepository)
        {
            _taskItemRepository = taskItemRepository;
        }

        public async Task<IEnumerable<TaskItemDTO>> Handle(GetByTitleQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
                throw new BadRequestException(nameof(query), "Query cannot be null");

            IEnumerable<TaskItem?> taskItems = await _taskItemRepository.GetTitleContains(query.UserId, query.Title, cancellationToken);
            if (taskItems == null || !taskItems.Any())
                throw new NotFoundException("TaskItems was not found");

            IEnumerable<TaskItemDTO> taskItemDTOs = taskItems.Select(TaskItemMapper.ToDTO);

            return taskItemDTOs;
        }
    }
}
