using BuildingBlocks.CQRS.Request;
using TaskManager.Application.DTOs.TaskLabel;

namespace TaskManager.Application.Features.TaskLabels.Queries.GetAll
{
    public record GetAllTaskLabelsQuery() : IRequest<GetAllTaskLabelsResult>;
    public record GetAllTaskLabelsResult(IEnumerable<TaskLabelDTO> TaskLabels);

}
