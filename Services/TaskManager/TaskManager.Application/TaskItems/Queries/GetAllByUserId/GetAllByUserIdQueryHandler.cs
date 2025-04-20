using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Mapping;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.TaskItems.Queries.GetAllByUserId
{
    public class GetAllByUserIdQueryHandler : IRequestHandler<GetAllByUserIdQuery, IEnumerable<TaskItemDTO>>
    {
        private readonly ITaskItemRepository _taskItemRepository;

        public GetAllByUserIdQueryHandler(ITaskItemRepository taskItemRepository)
        {
            _taskItemRepository = taskItemRepository;
        }

        public async Task<IEnumerable<TaskItemDTO>> Handle(GetAllByUserIdQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<TaskItem?> taskItems = await _taskItemRepository.GetByUserId(query.UserId, cancellationToken);
            if (taskItems == null || !taskItems.Any())
                throw new NotFoundException("TaskItems was not found");

            IEnumerable<TaskItemDTO> taskItemDTOs = taskItems.Select(TaskItemMapper.ToDTO);

            return taskItemDTOs;
        }
    }

}
