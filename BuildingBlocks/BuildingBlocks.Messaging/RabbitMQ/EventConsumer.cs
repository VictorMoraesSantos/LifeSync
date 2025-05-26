using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace BuildingBlocks.Messaging.RabbitMQ
{
    public class EventConsumer : IEventConsumer
    {
        private readonly PersistentConnection _persistentConnection;

        public EventConsumer(PersistentConnection persistentConnection)
        {
            _persistentConnection = persistentConnection;
        }

        public void StartConsuming(Action<string> onMessageReceived, ConsumerOptions? options = null)
        {
            options ??= new ConsumerOptions();

            _persistentConnection.ExecuteOnChannel(channel =>
            {
                channel.ExchangeDeclare(
                    exchange: options.ExchangeName,
                    type: options.TypeExchange,
                    durable: options.Durable,
                    autoDelete: options.AutoDelete,
                    arguments: options.Arguments);

                channel.QueueDeclare(
                    queue: options.QueueName,
                    durable: options.Durable,
                    exclusive: false,
                    autoDelete: options.AutoDelete,
                    arguments: options.Arguments);

                channel.QueueBind(
                    queue: options.QueueName,
                    exchange: options.ExchangeName,
                    routingKey: options.RoutingKey);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    onMessageReceived(message);

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                channel.BasicConsume(
                    queue: options.QueueName,
                    autoAck: false,
                    consumer: consumer);
            });
        }
    }
}