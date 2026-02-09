using LifeSyncApp.Models.TaskManager.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Concurrent;

namespace LifeSyncApp.Models.TaskManager
{
    public class TaskItem : INotifyPropertyChanged
    {
        private static readonly ConcurrentDictionary<string, PropertyChangedEventArgs> _eventArgsCache = new();

        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        private Status _status;
        public Status Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                    UpdateStatusProperties();
                }
            }
        }

        private Priority _priority;
        public Priority Priority
        {
            get => _priority;
            set
            {
                if (_priority != value)
                {
                    _priority = value;
                    OnPropertyChanged();
                    UpdatePriorityColor();
                }
            }
        }

        private DateOnly _dueDate;
        public DateOnly DueDate
        {
            get => _dueDate;
            set
            {
                if (_dueDate != value)
                {
                    _dueDate = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DueDateFormatted));
                    OnPropertyChanged(nameof(IsOverdue));
                }
            }
        }

        private List<TaskLabel> _labels = new();
        public List<TaskLabel> Labels
        {
            get => _labels;
            set
            {
                if (_labels != value)
                {
                    _labels = value;
                    OnPropertyChanged();
                }
            }
        }

        // ===== PROPRIEDADES COMPUTADAS PARA ELIMINAR CONVERTERS =====

        // Status properties
        private Color _statusColor = Colors.Gray;
        public Color StatusColor
        {
            get => _statusColor;
            private set
            {
                if (_statusColor != value)
                {
                    _statusColor = value;
                    OnPropertyChanged();
                }
            }
        }

        private Color _statusLightColor = Color.FromArgb("#F3F4F6");
        public Color StatusLightColor
        {
            get => _statusLightColor;
            private set
            {
                if (_statusLightColor != value)
                {
                    _statusLightColor = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _statusIcon = "";
        public string StatusIcon
        {
            get => _statusIcon;
            private set
            {
                if (_statusIcon != value)
                {
                    _statusIcon = value;
                    OnPropertyChanged();
                }
            }
        }

        // Priority property
        private Color _priorityColor = Color.FromArgb("#F59E0B");
        public Color PriorityColor
        {
            get => _priorityColor;
            private set
            {
                if (_priorityColor != value)
                {
                    _priorityColor = value;
                    OnPropertyChanged();
                }
            }
        }

        // Formatted dates
        public string DueDateFormatted => DueDate == DateOnly.FromDateTime(DateTime.Today)
            ? "Hoje"
            : DueDate == DateOnly.FromDateTime(DateTime.Today.AddDays(1))
            ? "Amanh\u00e3"
            : DueDate.ToString("dd/MM/yyyy");

        public string CreatedAtFormatted => CreatedAt.ToString("dd/MM/yyyy");

        public bool IsOverdue => DueDate < DateOnly.FromDateTime(DateTime.Today) && Status != Status.Completed;

        // ===== MÉTODOS PRIVADOS =====

        private void UpdateStatusProperties()
        {
            switch (Status)
            {
                case Status.Pending:
                    StatusColor = Color.FromArgb("#6B7280");
                    StatusLightColor = Color.FromArgb("#F3F4F6");
                    StatusIcon = "";
                    break;
                case Status.InProgress:
                    StatusColor = Color.FromArgb("#3B82F6");
                    StatusLightColor = Color.FromArgb("#DBEAFE");
                    StatusIcon = "\u25B6"; // Play icon
                    break;
                case Status.Completed:
                    StatusColor = Color.FromArgb("#10B981");
                    StatusLightColor = Color.FromArgb("#D1FAE5");
                    StatusIcon = "\u2713"; // Checkmark
                    break;
            }
        }

        private void UpdatePriorityColor()
        {
            PriorityColor = Priority switch
            {
                Priority.High => Color.FromArgb("#DC2626"),
                Priority.Medium => Color.FromArgb("#F59E0B"),
                Priority.Low => Color.FromArgb("#10B981"),
                _ => Color.FromArgb("#6B7280")
            };
        }

        // Constructor para inicializar propriedades computadas
        public TaskItem()
        {
            UpdateStatusProperties();
            UpdatePriorityColor();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return;
            var args = _eventArgsCache.GetOrAdd(propertyName, name => new PropertyChangedEventArgs(name));
            PropertyChanged?.Invoke(this, args);
        }
    }
}
