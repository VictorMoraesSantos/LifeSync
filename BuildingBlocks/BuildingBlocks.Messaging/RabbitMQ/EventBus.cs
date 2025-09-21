using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

public class EventBus : IEventBus
{
    private readonly PersistentConnection _conn;

    public EventBus(PersistentConnection conn)
    {
        _conn = conn;
    }

    public void PublishAsync<TEvent>(TEvent @event, PublishOptions? opts = null) where TEvent : IntegrationEvent
    {
        opts ??= new PublishOptions();

        using var channel = _conn.CreateModel();

        channel.ExchangeDeclare(
            exchange: opts.ExchangeName,
            type: opts.TypeExchange,
            durable: opts.Durable,
            autoDelete: opts.AutoDelete,
            arguments: opts.Arguments);

        var json = JsonConvert.SerializeObject(@event);
        var body = Encoding.UTF8.GetBytes(json);

        var props = channel.CreateBasicProperties();
        props.Persistent = true;

        channel.BasicPublish(
            exchange: opts.ExchangeName,
            routingKey: opts.RoutingKey,
            basicProperties: props,
            body: body);
    }
}