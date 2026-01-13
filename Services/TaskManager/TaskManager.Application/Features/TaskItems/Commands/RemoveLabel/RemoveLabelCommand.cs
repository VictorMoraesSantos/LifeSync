using BuildingBlocks.CQRS.Commands;

namespace TaskManager.Application.Features.TaskItems.Commands.RemoveLabel
{
    public record RemoveLabelCommand(int TaskItemId, List<int> TaskLabelsId) : ICommand<RemoveLabelResult>;
    public record RemoveLabelResult(bool IsSuccess);
}
