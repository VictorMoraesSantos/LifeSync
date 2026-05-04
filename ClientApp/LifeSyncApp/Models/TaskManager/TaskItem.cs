using CommunityToolkit.Mvvm.ComponentModel;
using LifeSyncApp.Models.TaskManager.Enums;

namespace LifeSyncApp.Models.TaskManager
{
    public partial class TaskItem : ObservableObject
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; }

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private Status _status;

        [ObservableProperty]
        private Priority _priority;

        [ObservableProperty]
        private DateOnly _dueDate;

        [ObservableProperty]
        private List<TaskLabel> _labels = new();
    }
}
