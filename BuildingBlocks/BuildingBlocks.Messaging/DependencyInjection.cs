using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace BuildingBlocks.Messaging
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqConfig = configuration.GetSection("RabbitMQ");

            var host = rabbitMqConfig["Host"] ?? "localhost";
            var port = int.TryParse(rabbitMqConfig["Port"], out var p) ? p : 5672;
            var userName = rabbitMqConfig["UserName"] ?? "guest";
            var password = rabbitMqConfig["Password"] ?? "guest";

            services.AddSingleton<IConnectionFactory>(sp =>
                new ConnectionFactory
                {
                    HostName = host,
                    Port = port,
                    UserName = userName,
                    Password = password
                });

            services.AddSingleton<PersistentConnection>();
            services.AddSingleton<IEventBus, EventBus>();
            services.AddSingleton<IEventConsumer, EventConsumer>();

            return services;
        }
    }
}