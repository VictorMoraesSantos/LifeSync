using CommunityToolkit.Mvvm.ComponentModel;

namespace LifeSyncApp.Models.TaskManager
{
    public partial class SelectableLabelItem : ObservableObject
    {
        public TaskLabel Label { get; set; }

        [ObservableProperty]
        private bool _isSelected;

        public SelectableLabelItem(TaskLabel label, bool isSelected = false)
        {
            Label = label;
            IsSelected = isSelected;
        }
    }
}
