using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Mapping;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.TaskItems.Queries.GetById
{
    public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, TaskItemDTO>
    {
        private readonly ITaskItemRepository _taskItemRepository;

        public GetByIdQueryHandler(ITaskItemRepository taskItemRepository)
        {
            _taskItemRepository = taskItemRepository;
        }

        public async Task<TaskItemDTO> Handle(GetByIdQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
                throw new BadRequestException("Query cannot be null");

            TaskItem? taskItem = await _taskItemRepository.GetById(query.Id, cancellationToken);
            if (taskItem == null)
                throw new NotFoundException(nameof(taskItem), query.Id);

            TaskItemDTO taskItemDTIO = taskItem.ToDTO();

            return taskItemDTIO;
        }
    }
}
