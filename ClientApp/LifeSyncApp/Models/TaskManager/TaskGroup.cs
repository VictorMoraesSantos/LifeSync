using System.Collections.ObjectModel;

namespace LifeSyncApp.Models.TaskManager
{
    public class TaskGroup : ObservableCollection<TaskItem>
    {
        public DateOnly DueDate { get; set; }
        public string DisplayDate { get; set; } = string.Empty;
        public bool IsOverdue { get; set; }
        public int TaskCount { get; set; }
        public string TaskCountText => TaskCount == 1 ? "1 tarefa" : $"{TaskCount} tarefas";

        public TaskGroup(DateOnly dueDate, IEnumerable<TaskItem> tasks) : base(tasks)
        {
            DueDate = dueDate;
            TaskCount = tasks.Count();
            IsOverdue = dueDate < DateOnly.FromDateTime(DateTime.Today);
            DisplayDate = FormatDueDate(dueDate);
        }

        private string FormatDueDate(DateOnly date)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var difference = date.DayNumber - today.DayNumber;

            if (difference == 0)
                return $"Hoje - {date:dd 'de' MMMM 'de' yyyy}";
            else if (difference == 1)
                return $"Amanhã - {date:dd 'de' MMMM 'de' yyyy}";
            else if (difference == -1)
                return $"Ontem - {date:dd 'de' MMMM 'de' yyyy} (Atrasada)";
            else if (difference < 0)
                return $"{date:dd 'de' MMMM 'de' yyyy} (Atrasada)";
            else
                return $"{date:dd 'de' MMMM 'de' yyyy}";
        }
    }
}
