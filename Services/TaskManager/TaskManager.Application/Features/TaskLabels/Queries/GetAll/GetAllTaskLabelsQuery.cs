using BuildingBlocks.CQRS.Queries;
using TaskManager.Application.DTOs.TaskLabel;

namespace TaskManager.Application.Features.TaskLabels.Queries.GetAll
{
    public record GetAllTaskLabelsQuery() : IQuery<GetAllTaskLabelsResult>;
    public record GetAllTaskLabelsResult(IEnumerable<TaskLabelDTO> TaskLabels);

}
