using LifeSyncApp.Client.Models.TaskManager.TaskItem;
using LifeSyncApp.Client.Models.TaskManager.TaskLabel;

namespace LifeSyncApp.Client.Components.Tasks.Extensions
{
    public static class TasksExtensions
    {
        public static string ToFriendlyDateString(this DateOnly date)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var tomorrow = today.AddDays(1);
            var yesterday = today.AddDays(-1);

            if (date == today)
                return $"Hoje - {date:dd/MM/yyyy}";
            else if (date == tomorrow)
                return $"Amanhã - {date:dd/MM/yyyy}";
            else if (date == yesterday)
                return $"Ontem - {date:dd/MM/yyyy}";
            else if (date < today)
                return $"Atrasado - {date:dd/MM/yyyy}";
            else
                return date.ToString("dd/MM/yyyy - dddd");
        }

        public static string GetPriorityColor(Priority priority)
        {
            return priority switch
            {
                Priority.High => "error",
                Priority.Urgent => "error",
                Priority.Medium => "warning",
                Priority.Low => "secondary",
                _ => "primary"
            };
        }

        public static string GetStatusColor(Status status)
        {
            return status switch
            {
                Status.Completed => "success",
                Status.InProgress => "warning",
                Status.Pending => "secondary",
                Status.Cancelled => "error",
                _ => "secondary"
            };
        }

        public static string GetLabelHex(LabelColor color)
        {
            return color switch
            {
                LabelColor.Red => "#FF0000",
                LabelColor.Green => "#00FF00",
                LabelColor.Blue => "#0000FF",
                LabelColor.Yellow => "#FFFF00",
                LabelColor.Purple => "#800080",
                LabelColor.Orange => "#FFA500",
                LabelColor.Pink => "#FFC0CB",
                LabelColor.Brown => "#A52A2A",
                LabelColor.Gray => "#808080",
                _ => "#777777",
            };
        }

        public static string GetLabelTextColor(LabelColor color)
        {
            return color switch
            {
                LabelColor.Yellow => "#000000",
                LabelColor.Pink => "#000000",
                _ => "#FFFFFF",
            };
        }


    }
}
