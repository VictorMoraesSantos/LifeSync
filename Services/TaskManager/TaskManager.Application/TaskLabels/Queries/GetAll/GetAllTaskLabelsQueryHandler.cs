using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Mapping;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.TaskLabels.Queries.GetAll
{
    public class GetAllTaskLabelsQueryHandler : IRequestHandler<GetAllTaskLabelsQuery, IEnumerable<TaskLabelDTO>>
    {
        private readonly ITaskLabelRepository _taskLabelRepository;

        public GetAllTaskLabelsQueryHandler(ITaskLabelRepository taskLabelRepository)
        {
            _taskLabelRepository = taskLabelRepository;
        }

        public async Task<IEnumerable<TaskLabelDTO>> Handle(GetAllTaskLabelsQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<TaskLabel?> taskLabels = await _taskLabelRepository.GetAll(cancellationToken);
            if (taskLabels == null)
                throw new NotFoundException("TaskLabel was not found");

            IEnumerable<TaskLabelDTO> taskLabelDTOs = taskLabels.Select(tl => tl.ToDTO());

            return taskLabelDTOs;
        }
    }
}
