using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Options;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.RabbitMQ
{
    public static class EventConsumerExtensions
    {
        public static IServiceCollection AddEventConsumer<TEvent>(
            this IServiceCollection services,
            Action<ConsumerOptions> configure
        ) where TEvent : IntegrationEvent
        {
            var opts = new ConsumerOptions();
            configure(opts);
            services.AddSingleton<IConsumerDefinition>(
                new ConsumerDefinition(typeof(TEvent), opts)
            );
            return services;
        }
    }
}
