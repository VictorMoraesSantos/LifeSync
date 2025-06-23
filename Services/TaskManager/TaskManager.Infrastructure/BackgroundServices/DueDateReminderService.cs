using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Events;

namespace TaskManager.Application.BackgroundServices
{
    public class DueDateReminderService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IEventBus _eventBus;
        private readonly TimeSpan _threshold = TimeSpan.FromDays(1);
        private readonly TimeSpan _pollingInterval = TimeSpan.FromHours(1);

        public DueDateReminderService(
            IServiceScopeFactory scopeFactory,
            IEventBus eventBus)
        {
            _scopeFactory = scopeFactory;
            _eventBus = eventBus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var taskService = scope.ServiceProvider.GetRequiredService<ITaskItemService>();
                var now = DateTime.UtcNow;

                // 1) Busque as tasks a lembrar
                var dueTaskDtos = await taskService.FindAsync(
                    t =>
                        t.DueDate.ToDateTime(TimeOnly.MinValue) > now
                     && t.DueDate.ToDateTime(TimeOnly.MinValue) <= now.Add(_threshold),
                    stoppingToken);

                foreach (var task in dueTaskDtos)
                {
                    // 2) Monte o evento
                    var @event = new TaskDueReminderEvent(
                        task.Id,
                        task.UserId,
                        task.Title,
                        task.DueDate);

                    // 3) Configure as opções de publicação
                    var options = new PublishOptions
                    {
                        ExchangeName = "task_exchange",
                        TypeExchange = ExchangeType.Topic,
                        RoutingKey = "task.due.reminder",
                        Durable = true,
                    };

                    // 4) Publique na fila
                    _eventBus.PublishAsync(@event, options);
                }
                // 5) Aguarde o próximo loop
                await Task.Delay(_pollingInterval, stoppingToken);
            }
        }
    }
}