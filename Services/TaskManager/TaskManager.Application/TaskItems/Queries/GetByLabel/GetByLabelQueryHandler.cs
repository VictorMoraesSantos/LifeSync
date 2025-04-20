using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Mapping;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.TaskItems.Queries.GetByLabel
{
    public class GetByLabelQueryHandler : IRequestHandler<GetByLabelQuery, IEnumerable<TaskItemDTO>>
    {
        private readonly ITaskItemRepository _taskItemRepository;

        public GetByLabelQueryHandler(ITaskItemRepository taskItemRepository)
        {
            _taskItemRepository = taskItemRepository;
        }

        public async Task<IEnumerable<TaskItemDTO>> Handle(GetByLabelQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
                throw new BadRequestException(nameof(query), "Query cannot be null");

            TaskLabel label = query.Label.ToEntity();

            IEnumerable<TaskItem?> taskItems = await _taskItemRepository.GetByLabel(query.UserId, label, cancellationToken);
            if (taskItems == null || !taskItems.Any())
                throw new NotFoundException("TaskItems was not found");

            IEnumerable<TaskItemDTO> taskItemDTOs = taskItems.Select(TaskItemMapper.ToDTO);

            return taskItemDTOs;
        }
    }
}
