using BuildingBlocks.CQRS.Queries;
using TaskManager.Application.DTOs.TaskLabel;

namespace TaskManager.Application.Features.TaskLabels.Queries.GetById
{
    public record GetTaskLabelByIdQuery(int Id) : IQuery<GetTaskLabelByIdResult>;
    public record GetTaskLabelByIdResult(TaskLabelDTO? TaskLabel);
}
