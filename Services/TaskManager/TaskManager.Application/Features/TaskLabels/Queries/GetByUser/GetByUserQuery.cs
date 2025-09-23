using BuildingBlocks.CQRS.Queries;
using TaskManager.Application.DTOs.TaskLabel;

namespace TaskManager.Application.Features.TaskLabels.Queries.GetByUser
{
    public record GetByUserQuery(int UserId) : IQuery<GetByUserResponse>;
    public record GetByUserResponse(IEnumerable<TaskLabelDTO> TaskLabels);
}
