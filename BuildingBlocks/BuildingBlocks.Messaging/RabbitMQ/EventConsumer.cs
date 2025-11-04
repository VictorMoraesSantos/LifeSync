using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace BuildingBlocks.Messaging.RabbitMQ
{
    public class EventConsumer : IEventConsumer, IDisposable
    {
        private readonly PersistentConnection _persistentConnection;
        private IChannel? _channel;

        public EventConsumer(PersistentConnection persistentConnection)
        {
            _persistentConnection = persistentConnection;
            _channel = _persistentConnection.CreateChannel();
        }

        public void StartConsuming(Action<string> onMessageReceived, ConsumerOptions? options = null)
        {
            options ??= new ConsumerOptions();

            if (_channel is null || _channel.IsClosed)
            {
                _channel?.Dispose();
                _channel = _persistentConnection.CreateChannel();
            }

            _channel.ExchangeDeclareAsync(
                exchange: options.ExchangeName,
                type: options.TypeExchange,
                durable: options.Durable,
                autoDelete: options.AutoDelete,
                arguments: options.Arguments).GetAwaiter().GetResult();

            _channel.QueueDeclareAsync(
                queue: options.QueueName,
                durable: options.Durable,
                exclusive: false,
                autoDelete: options.AutoDelete,
                arguments: options.Arguments).GetAwaiter().GetResult();

            _channel.QueueBindAsync(
                queue: options.QueueName,
                exchange: options.ExchangeName,
                routingKey: options.RoutingKey).GetAwaiter().GetResult();

            _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false).GetAwaiter().GetResult();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (ch, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                onMessageReceived(json);
                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsumeAsync(
                queue: options.QueueName,
                autoAck: false,
                consumer: consumer).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            _channel?.CloseAsync().GetAwaiter().GetResult();
            _channel?.Dispose();
            _channel = null;
        }
    }
}