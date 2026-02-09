using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LifeSyncApp.Models.TaskManager
{
    public class SelectableLabelItem : INotifyPropertyChanged
    {
        public TaskLabel Label { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SelectableLabelItem(TaskLabel label, bool isSelected = false)
        {
            Label = label;
            IsSelected = isSelected;
        }
    }
}
