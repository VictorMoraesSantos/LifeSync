namespace LifeSyncApp.Client.Models.TaskManager.TaskLabel
{
    public class TaskLabelDTO
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Name { get; set; }
        public LabelColor Color { get; set; }
        public int UserId { get; set; }
        public int TaskItemId { get; set; }
    }
}
