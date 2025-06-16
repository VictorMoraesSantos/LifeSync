using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Options;

namespace BuildingBlocks.Messaging.RabbitMQ
{
    public class ConsumerDefinition : IConsumerDefinition
    {
        public Type EventType { get; }
        public ConsumerOptions Options { get; }

        public ConsumerDefinition(Type eventType, ConsumerOptions options)
        {
            EventType = eventType;
            Options = options;
        }
    }
}
