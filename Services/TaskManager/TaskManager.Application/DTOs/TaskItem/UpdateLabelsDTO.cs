namespace TaskManager.Application.DTOs.TaskItem
{
    public record UpdateLabelsDTO(int TaskItemId, List<int> TaskLabelsId);
}
