using BuildingBlocks.CQRS.Notification;
using BuildingBlocks.CQRS.Publisher;
using BuildingBlocks.CQRS.Request;
using BuildingBlocks.CQRS.Sender;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks.CQRS.Extensions
{
    public static class Addsender
    {
        public static IServiceCollection AddMediatorService(this IServiceCollection services, Assembly? assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();
            services.AddScoped<ISender, BuildingBlocks.CQRS.Sender.Sender>();
            services.AddScoped<IPublisher, BuildingBlocks.CQRS.Publisher.Publisher>();

            var handlerInterfaceType = typeof(IRequestHandler<,>);
            var notificationHandlerInterfaceType = typeof(INotificationHandler<>);

            var types = assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface);

            var handlerTypes = types
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType)
                    .Select(i => new { Interface = i, Implementation = t }));

            foreach (var handler in handlerTypes)
            {
                services.AddScoped(handler.Interface, handler.Implementation);
            }

            var notificationHandlerTypes = types
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == notificationHandlerInterfaceType)
                    .Select(i => new { Interface = i, Implementation = t }));

            foreach (var handler in notificationHandlerTypes)
            {
                services.AddScoped(handler.Interface, handler.Implementation);
            }

            return services;
        }
    }
}