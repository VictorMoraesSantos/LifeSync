using LifeSyncApp.Client.Models.TaskManager.TaskLabel;
using System.Threading.Channels;

namespace LifeSyncApp.Client.Models.TaskManager.TaskItem
{
    public class TaskItemDTO
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public DateOnly DueDate { get; set; }
        public int UserId { get; set; }
        public List<TaskLabelDTO> Labels { get; set; }
    }
}
