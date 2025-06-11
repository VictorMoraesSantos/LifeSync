using BuildingBlocks.CQRS.Publisher;
using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Options;
using EmailSender.Domain.Events;
using RabbitMQ.Client;
using System.Text.Json;

namespace EmailSender.Infrastructure.Messaging
{
    public class RabbitMqEventConsumer
    {
        private readonly IEventConsumer _eventConsumer;
        private readonly IPublisher _publisher;

        public RabbitMqEventConsumer(
            IEventConsumer eventConsumer,
            IPublisher publisher)
        {
            _eventConsumer = eventConsumer;
            _publisher = publisher;
        }

        public void StartConsuming()
        {
            var options = new ConsumerOptions
            {
                QueueName = "email_events",
                ExchangeName = "my_exchange",
                RoutingKey = "email.events.user.registered", // Rota específica
                TypeExchange = ExchangeType.Topic
            };

            _eventConsumer.StartConsuming(OnMessageReceived, options);
        }

        private void OnMessageReceived(string message)
        {
            // Tenta desserializar para UserRegisteredEvent
            var @event = JsonSerializer.Deserialize<UserRegisteredEvent>(message);

            // Publica o evento de domínio usando MediatR
            _publisher.Publish(@event);
        }
    }
}