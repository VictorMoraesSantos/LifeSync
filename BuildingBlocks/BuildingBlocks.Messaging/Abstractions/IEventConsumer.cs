using BuildingBlocks.Messaging.Options;

namespace BuildingBlocks.Messaging.Abstractions
{
    public interface IEventConsumer
    {
        void StartConsuming(Action<string> onMessageReceived, ConsumerOptions? options = null);
    }
}
