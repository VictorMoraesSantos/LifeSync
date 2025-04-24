using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.TaskLabels.Queries.GetAll
{
    public record GetAllTaskLabelsQuery() : IRequest<GetAllTaskLabelsResponse>;
    public record GetAllTaskLabelsResponse(IEnumerable<TaskLabelDTO> TaskLabelDTOs);

}
