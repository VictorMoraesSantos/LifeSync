using BuildingBlocks.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Application.Features.TaskItems.Commands.RemoveLabel
{
    public record RemoveLabelCommand(int TaskItemId, List<int> TaskLabelsId): ICommand<RemoveLabelResult>;
    public record RemoveLabelResult(bool IsSuccess);
}
