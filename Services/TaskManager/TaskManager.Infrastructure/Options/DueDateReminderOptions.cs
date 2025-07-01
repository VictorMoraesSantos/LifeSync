namespace TaskManager.Infrastructure.Options
{
    // Classe de opções para configuração
    public class DueDateReminderOptions
    {
        public TimeSpan ReminderThreshold { get; set; } = TimeSpan.FromDays(1);
        public TimeSpan PollingInterval { get; set; } = TimeSpan.FromHours(1);
        public string ExchangeName { get; set; } = "task_exchange";
        public string RoutingKey { get; set; } = "task.due.reminder";
        public int MaxTasksPerRun { get; set; } = 100;
    }
}
