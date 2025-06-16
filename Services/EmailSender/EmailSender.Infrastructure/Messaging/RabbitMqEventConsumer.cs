using BuildingBlocks.CQRS.Publisher;
using BuildingBlocks.Messaging.Abstractions;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

public class RabbitMqEventConsumer : BackgroundService
{
    private readonly IEventConsumer _rawConsumer;
    private readonly IPublisher _publisher;
    private readonly IEnumerable<IConsumerDefinition> _defs;

    public RabbitMqEventConsumer(
        IEventConsumer rawConsumer,
        IPublisher publisher,
        IEnumerable<IConsumerDefinition> defs)
    {
        _rawConsumer = rawConsumer;
        _publisher = publisher;
        _defs = defs;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var def in _defs)
        {
            void OnMessage(string json)
            {
                var @event = (IntegrationEvent)JsonSerializer
                    .Deserialize(json, def.EventType,
                                 new JsonSerializerOptions
                                 {
                                     PropertyNameCaseInsensitive = true
                                 })!;

                _publisher
                  .Publish(@event, stoppingToken)
                  .GetAwaiter()
                  .GetResult();
            }

            _rawConsumer.StartConsuming(OnMessage, def.Options);
        }

        return Task.CompletedTask;
    }
}