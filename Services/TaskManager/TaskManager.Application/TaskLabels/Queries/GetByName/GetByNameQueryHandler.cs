using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Mapping;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.TaskLabels.Queries.GetByName
{
    public class GetByNameQueryHandler : IRequestHandler<GetByNameQuery, IEnumerable<TaskLabelDTO>?>
    {
        private readonly ITaskLabelRepository _taskLabelRepository;

        public GetByNameQueryHandler(ITaskLabelRepository taskLabelRepository)
        {
            _taskLabelRepository = taskLabelRepository;
        }

        public async Task<IEnumerable<TaskLabelDTO>?> Handle(GetByNameQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<TaskLabel?> taskLabels = await _taskLabelRepository.GetByName(query.UserId, query.Name, cancellationToken);
            if (taskLabels == null || !taskLabels.Any())
                throw new NotFoundException("TaskLabel cannot be found");

            IEnumerable<TaskLabelDTO> taskLabelDTOs = taskLabels.Select(t => t.ToDTO());

            return taskLabelDTOs;
        }
    }
}
