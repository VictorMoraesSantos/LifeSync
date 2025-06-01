using BuildingBlocks.CQRS.Notification;

namespace BuildingBlocks.CQRS.Publisher
{
    public interface IPublisher
    {
        Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification;
    }
}
