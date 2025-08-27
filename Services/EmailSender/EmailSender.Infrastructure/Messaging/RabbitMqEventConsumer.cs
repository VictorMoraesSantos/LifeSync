using BuildingBlocks.CQRS.Publisher;
using BuildingBlocks.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace EmailSender.Infrastructure.Messaging;
public class RabbitMqEventConsumer : BackgroundService
{
    private readonly IEventConsumer _rawConsumer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEnumerable<IConsumerDefinition> _defs;

    public RabbitMqEventConsumer(
        IEventConsumer rawConsumer,
        IServiceScopeFactory scopeFactory,
        IEnumerable<IConsumerDefinition> defs)
    {
        _rawConsumer = rawConsumer;
        _scopeFactory = scopeFactory;
        _defs = defs;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var def in _defs)
        {
            void OnMessage(string json)
            {
                using var scope = _scopeFactory.CreateScope();
                var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

                var @event = (IntegrationEvent)JsonSerializer.Deserialize(
                    json,
                    def.EventType,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

                // publica dentro do escopo
                publisher.Publish(@event, stoppingToken).GetAwaiter().GetResult();
            }

            _rawConsumer.StartConsuming(OnMessage, def.Options);
        }

        return Task.CompletedTask;
    }
}