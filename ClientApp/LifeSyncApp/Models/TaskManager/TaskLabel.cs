using LifeSyncApp.Models.TaskManager.Enums;
using System.ComponentModel;

namespace LifeSyncApp.Models.TaskManager
{
    public class TaskLabel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Name { get; set; } = string.Empty;
        public LabelColor LabelColor { get; set; }
        public int UserId { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
