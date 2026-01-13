namespace LifeSyncApp.Models.TaskManager.Enums
{
    public enum Status
    {
        Pending = 1,
        InProgress = 2,
        Completed = 3,
        Cancelled = 4,
    }

    public static class StatusExtensions
    {
        public static string ToFriendlyString(this Status status)
        {
            return status switch
            {
                Status.Pending => "Pendente",
                Status.InProgress => "Em progresso",
                Status.Completed => "Completado",
                Status.Cancelled => "Cancelado",
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
            };
        }
    }
}
