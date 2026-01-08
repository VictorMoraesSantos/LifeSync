using BuildingBlocks.CQRS.Commands;

namespace TaskManager.Application.Features.TaskItems.Commands.AddLabel
{
    public record AddLabelCommand(int TaskItemId, List<int> TaskLabelsId) : ICommand<AddLabelResult>;
    public record AddLabelResult(bool IsSuccess);
}
