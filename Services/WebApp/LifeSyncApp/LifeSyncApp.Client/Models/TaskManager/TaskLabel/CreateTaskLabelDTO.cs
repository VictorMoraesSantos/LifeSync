namespace LifeSyncApp.Client.Models.TaskManager.TaskLabel
{
    public class CreateTaskLabelDTO
    {
        public string Name { get; set; } = string.Empty;
        public LabelColor LabelColor { get; set; }
        public int UserId { get; set; }
        public int TaskItemId { get; set; }

        public CreateTaskLabelDTO(string name, LabelColor labelColor, int userId, int taskItemId)
        {
            Name = name;
            LabelColor = labelColor;
            UserId = userId;
            TaskItemId = taskItemId;
        }
    }
}
