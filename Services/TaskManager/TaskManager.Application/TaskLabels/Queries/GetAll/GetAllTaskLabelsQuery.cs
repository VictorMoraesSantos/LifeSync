using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.TaskLabels.Queries.GetAll
{
    public record GetAllTaskLabelsQuery() : IRequest<GetAllTaskLabelsResult>;
    public record GetAllTaskLabelsResult(IEnumerable<TaskLabelDTO> TaskLabelDTOs);

}
