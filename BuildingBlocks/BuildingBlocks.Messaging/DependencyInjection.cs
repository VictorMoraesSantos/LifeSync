﻿using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.RabbitMQ;
using BuildingBlocks.Messaging.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace BuildingBlocks.Messaging
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqSettings>(options => 
                configuration.GetSection("RabbitMQSettings").Bind(options));

            services.AddSingleton<IConnectionFactory>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
                return new ConnectionFactory
                {
                    HostName = options.Host,
                    Port = options.Port,
                    UserName = options.User,
                    Password = options.Password,
                    DispatchConsumersAsync = true
                };
            });

            services.AddSingleton<PersistentConnection>();
            services.AddSingleton<IEventBus, EventBus>();
            services.AddSingleton<IEventConsumer, EventConsumer>();

            return services;
        }
    }
}