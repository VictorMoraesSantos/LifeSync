using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Mapping;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.TaskLabels.Queries.GetById
{
    public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, TaskLabelDTO>
    {
        private readonly ITaskLabelRepository _taskLabelRepository;

        public GetByIdQueryHandler(ITaskLabelRepository taskLabelRepository)
        {
            _taskLabelRepository = taskLabelRepository;
        }

        public async Task<TaskLabelDTO> Handle(GetByIdQuery query, CancellationToken cancellationToken)
        {
            if(query == null)
                throw new BadRequestException("Query cannot be null");

            TaskLabel? taskLabel = await _taskLabelRepository.GetById(query.Id, cancellationToken);

            TaskLabelDTO? taskLabelDTO = TaskLabelMapper.ToDTO(taskLabel);
            
            return taskLabelDTO;
        }
    }
}
