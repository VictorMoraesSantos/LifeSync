using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace BuildingBlocks.Messaging.RabbitMQ
{
    public class EventConsumer : IEventConsumer, IDisposable
    {
        private readonly IModel _channel;

        public EventConsumer(PersistentConnection persistentConnection)
        {
            _channel = persistentConnection.CreateModel();
        }

        public void StartConsuming(Action<string> onMessageReceived, ConsumerOptions? options = null)
        {
            options ??= new ConsumerOptions();

            _channel.ExchangeDeclare(
                exchange: options.ExchangeName,
                type: options.TypeExchange,
                durable: options.Durable,
                autoDelete: options.AutoDelete,
                arguments: options.Arguments);

            _channel.QueueDeclare(
                queue: options.QueueName,
                durable: options.Durable,
                exclusive: false,
                autoDelete: options.AutoDelete,
                arguments: options.Arguments);

            _channel.QueueBind(
                queue: options.QueueName,
                exchange: options.ExchangeName,
                routingKey: options.RoutingKey);

            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                onMessageReceived(json);
                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(
                queue: options.QueueName,
                autoAck: false,
                consumer: consumer);
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
        }
    }
}