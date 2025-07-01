using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Events;
using TaskManager.Infrastructure.Options;

namespace TaskManager.Application.BackgroundServices
{
    public class DueDateReminderService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IEventBus _eventBus;
        private readonly ILogger<DueDateReminderService> _logger;
        private readonly DueDateReminderOptions _options;

        public DueDateReminderService(
            IServiceScopeFactory scopeFactory,
            IEventBus eventBus,
            ILogger<DueDateReminderService> logger,
            IOptions<DueDateReminderOptions> options)
        {
            _scopeFactory = scopeFactory;
            _eventBus = eventBus;
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando o serviço de lembretes de tarefas com vencimento próximo");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessDueTasksAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar lembretes de tarefas com vencimento próximo");
                }

                await Task.Delay(_options.PollingInterval, cancellationToken);
            }

            _logger.LogInformation("Serviço de lembretes de tarefas encerrado");
        }

        private async Task ProcessDueTasksAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var taskService = scope.ServiceProvider.GetRequiredService<ITaskItemService>();
            var now = DateTime.UtcNow;
            var thresholdDate = now.Add(_options.ReminderThreshold);

            _logger.LogDebug("Buscando tarefas com vencimento próximo (entre {Now} e {ThresholdDate})",
                now, thresholdDate);

            var result = await taskService.FindAsync(
                t => t.DueDate.ToDateTime(TimeOnly.MinValue) > now
                  && t.DueDate.ToDateTime(TimeOnly.MinValue) <= thresholdDate,
                cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Falha ao buscar tarefas com vencimento próximo: {ErrorMessage}",
                    result.Error);
                return;
            }

            var dueTasks = result.Value;
            if (dueTasks == null || !dueTasks.Any())
            {
                _logger.LogDebug("Nenhuma tarefa com vencimento próximo encontrada");
                return;
            }

            _logger.LogInformation("Encontradas {Count} tarefas com vencimento próximo", dueTasks.Count());

            var publishOptions = new PublishOptions
            {
                ExchangeName = _options.ExchangeName,
                TypeExchange = ExchangeType.Topic,
                RoutingKey = _options.RoutingKey,
                Durable = true,
            };

            int count = 0;
            foreach (var task in dueTasks.Take(_options.MaxTasksPerRun))
            {
                try
                {
                    var @event = new TaskDueReminderEvent(
                        task.Id,
                        task.UserId,
                        task.Title,
                        task.DueDate);

                    _eventBus.PublishAsync(@event, publishOptions);

                    // Atualize o status de lembrete enviado (se tiver implementação para isso)
                    // await taskService.MarkReminderSentAsync(task.Id, stoppingToken);

                    count++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao publicar lembrete para a tarefa {TaskId}", task.Id);
                }
            }

            _logger.LogInformation("Publicados {Count} lembretes de tarefas com vencimento próximo", count);
        }
    }


}