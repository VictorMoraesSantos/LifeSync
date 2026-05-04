using CommunityToolkit.Mvvm.ComponentModel;
using LifeSyncApp.Models.TaskManager.Enums;

namespace LifeSyncApp.Models.TaskManager
{
    public partial class TaskLabel : ObservableObject
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private LabelColor _labelColor;

        public int UserId { get; set; }
    }
}
