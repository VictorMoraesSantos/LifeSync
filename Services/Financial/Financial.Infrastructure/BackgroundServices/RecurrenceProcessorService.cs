using Financial.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Financial.Infrastructure.BackgroundServices
{
    public class RecurrenceProcessorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RecurrenceProcessorService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromHours(1);

        public RecurrenceProcessorService(IServiceScopeFactory scopeFactory, ILogger<RecurrenceProcessorService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço de processamento de recorrência iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<IRecurrenceScheduleService>();

                    var result = await service.ProcessDueSchedulesAsync(stoppingToken);
                    if (!result.IsSuccess)
                    {
                        _logger.LogWarning("Falha no processamento: {Error}", result.Error?.Description);
                        continue;
                    }

                    _logger.LogInformation("Processamento concluído: {Count} transações geradas", result.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar recorrências");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
