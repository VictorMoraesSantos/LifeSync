using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

public class EventBus : IEventBus
{
    private readonly PersistentConnection _persistentConnection;

    public EventBus(PersistentConnection persistentConnection)
    {
        _persistentConnection = persistentConnection;
    }

    public void PublishAsync<TEvent>(TEvent @event, PublishOptions? options = null) where TEvent : IntegrationEvent
    {
        options ??= new PublishOptions();

        _persistentConnection.ExecuteOnChannel(channel =>
        {
            channel.ExchangeDeclare(
                exchange: options.ExchangeName,
                type: options.TypeExchange,
                durable: options.Durable,
                autoDelete: options.AutoDelete,
                arguments: options.Arguments);

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
                exchange: options.ExchangeName,
                routingKey: options.RoutingKey,
                basicProperties: properties,
                body: body);
        });
    }
}