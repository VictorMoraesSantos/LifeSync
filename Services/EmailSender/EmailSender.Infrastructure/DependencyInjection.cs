using BuildingBlocks.CQRS.Publisher;
using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.RabbitMQ;
using BuildingBlocks.Messaging.Settings;
using EmailSender.Application.Contracts;
using EmailSender.Domain.Events;
using EmailSender.Infrastructure.Services;
using EmailSender.Infrastructure.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System.Net.Mail;

namespace EmailSender.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEmailSenderInfrastructure(configuration);
            services.AddMessaging(configuration);
            return services;
        }

        public static IServiceCollection AddEmailSenderInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var smtpSettings = configuration.GetSection("SmtpSettings")
                                            .Get<SmtpSettings>();
            services.AddSingleton(sp =>
                new SmtpClient(smtpSettings.Host, smtpSettings.Port)
                {
                    Credentials = new System.Net.NetworkCredential(smtpSettings.User, smtpSettings.Password),
                    EnableSsl = smtpSettings.EnableSsl
                });
            services.AddScoped<IEmailSender, SmtpEmailSender>();


            return services;
        }

        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitCfg = configuration.GetSection("RabbitMQSettings").Get<RabbitMqSettings>();
            services.AddSingleton<IConnectionFactory>(sp =>
                new ConnectionFactory()
                {
                    HostName = rabbitCfg.Host,
                    UserName = rabbitCfg.User,
                    Password = rabbitCfg.Password,
                    Port = rabbitCfg.Port,
                    DispatchConsumersAsync = false
                });
            services.AddSingleton<PersistentConnection>();
            services.AddSingleton<IEventConsumer, EventConsumer>();
            services.AddScoped<IPublisher, Publisher>();

            services.AddEventConsumer<UserRegisteredIntegrationEvent>(opts =>
            {
                opts.ExchangeName = "user_exchange";
                opts.QueueName = "email_events.user_registered";
                opts.RoutingKey = "user.registered";
                opts.TypeExchange = ExchangeType.Topic;
                opts.Durable = true;
                opts.AutoDelete = false;
            });

            services.AddHostedService<RabbitMqEventConsumer>();

            return services;
        }
    }
}