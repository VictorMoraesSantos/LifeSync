using BuildingBlocks.CQRS.Notification;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.CQRS.Publisher
{
    public class Publisher : IPublisher
    {
        private readonly IServiceProvider _serviceProvider;

        public Publisher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
            var handlers = _serviceProvider.GetServices(handlerType);
            var tasks = handlers
                .Cast<object>()
                .Select(handler =>
                {
                    var method = handlerType.GetMethod("Handle");
                    return (Task?)method?.Invoke(handler, new object[] { notification, cancellationToken });
                });

            await Task.WhenAll(tasks!);
        }
    }
}
