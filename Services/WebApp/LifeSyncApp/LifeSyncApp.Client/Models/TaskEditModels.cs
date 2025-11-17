using System;
using LifeSyncApp.Client.Models.TaskManager.TaskItem;
using LifeSyncApp.Client.Models.TaskManager.TaskLabel;

namespace LifeSyncApp.Client.Models.Components
{
    public class TaskEditModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Status Status { get; set; } = Status.Pending;
        public Priority Priority { get; set; } = Priority.Medium;
        public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }

    public class LabelEditModel
    {
        public string Name { get; set; } = string.Empty;
        public LabelColor Color { get; set; } = LabelColor.Purple;
    }
}
