using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Application.DTOs.TaskItem
{
    public record UpdateLabelsDTO(int TaskItemId, List<int> TaskLabelsId);
}
