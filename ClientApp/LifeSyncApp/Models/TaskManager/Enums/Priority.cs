namespace LifeSyncApp.Models.TaskManager.Enums
{
    public enum Priority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Urgent = 4
    }

    public static class PriorityExtensions
    {
        public static string ToFriendlyString(this Priority priority)
        {
            return priority switch
            {
                Priority.Low => "Baixa",
                Priority.Medium => "Média",
                Priority.High => "Alta",
                Priority.Urgent => "Urgente",
                _ => throw new ArgumentOutOfRangeException(nameof(priority), priority, null)
            };
        }
    }
}
