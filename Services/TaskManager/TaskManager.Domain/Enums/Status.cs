namespace TaskManager.Domain.Enums
{
    public enum Status
    {
        Pending = 1,
        InProgress = 2,
        Completed = 3,
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
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
            };
        }
    }
}
