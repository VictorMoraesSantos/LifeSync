using BuildingBlocks.Messaging.Options;

namespace BuildingBlocks.Messaging.Abstractions
{
    public interface IEventBus
    {
        void Publish<T>(T @event, PublishOptions? options = null) where T : IntegrationEvent;
    }
}
