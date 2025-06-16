using BuildingBlocks.Messaging.Options;

namespace BuildingBlocks.Messaging.Abstractions
{
    public interface IEventBus
    {
        void PublishAsync<TEvent>(
            TEvent @event,
            PublishOptions options)
            where TEvent : IntegrationEvent;

    }
}
