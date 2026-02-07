using LifeSyncApp.Models.TaskManager.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LifeSyncApp.Models.TaskManager
{
    public class TaskLabel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        private LabelColor _labelColor;
        public LabelColor LabelColor
        {
            get => _labelColor;
            set
            {
                if (_labelColor != value)
                {
                    _labelColor = value;
                    OnPropertyChanged();
                }
            }
        }

        public int UserId { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
