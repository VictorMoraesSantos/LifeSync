using BuildingBlocks.Messaging.Options;

namespace BuildingBlocks.Messaging.Abstractions
{
    public interface IConsumerDefinition
    {
        Type EventType { get; }
        ConsumerOptions Options { get; }
    }
}
