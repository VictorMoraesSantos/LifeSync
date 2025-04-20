using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Mapping;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.TaskLabels.Queries.GetByUserId
{
    public class GetByUserIdQueryHandler : IRequestHandler<GetByUserIdQuery, IEnumerable<TaskLabelDTO>?>
    {
        private readonly ITaskLabelRepository _taskLabelRepository;
        public GetByUserIdQueryHandler(ITaskLabelRepository taskLabelRepository)
        {
            _taskLabelRepository = taskLabelRepository;
        }

        public async Task<IEnumerable<TaskLabelDTO>?> Handle(GetByUserIdQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<TaskLabel?> taskLabels = await _taskLabelRepository.GetByUserId(request.UserId, cancellationToken);
            if (taskLabels == null || !taskLabels.Any())
                throw new NotFoundException("TaskLabel cannot be found");

            IEnumerable<TaskLabelDTO> taskLabelDTOs = taskLabels.Select(t => t.ToDTO());

            return taskLabelDTOs;
        }
    }
}
