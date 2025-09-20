using BuildingBlocks.Messaging.RabbitMQ;
using BuildingBlocks.Messaging;
using EmailSender.Application.Contracts;
using EmailSender.Domain.Events;
using EmailSender.Infrastructure.Messaging;
using EmailSender.Infrastructure.Services;
using EmailSender.Infrastructure.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace EmailSender.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEmailSenderInfrastructure(configuration);
            services.AddMessaging(configuration);
            services.AddEventConsumers();
            return services;
        }

        public static IServiceCollection AddEmailSenderInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
            services.AddScoped<IEmailSender, SmtpEmailSender>();
            return services;
        }

        public static IServiceCollection AddEventConsumers(this IServiceCollection services)
        {
            services.AddEventConsumer<UserRegisteredIntegrationEvent>(opts =>
            {
                opts.ExchangeName = "user_exchange";
                opts.QueueName = "email_events.user_registered";
                opts.RoutingKey = "user.registered";
                opts.TypeExchange = ExchangeType.Topic;
                opts.Durable = true;
                opts.AutoDelete = false;
            });

            services.AddEventConsumer<TaskDueReminderIntegrationEvent>(opts =>
            {
                opts.ExchangeName = "task_exchange";
                opts.QueueName = "task_events.task_reminder";
                opts.RoutingKey = "task.due.reminder";
                opts.TypeExchange = ExchangeType.Topic;
                opts.Durable = true;
                opts.AutoDelete = false;
            });

            services.AddHostedService<RabbitMqEventConsumer>();

            return services;
        }
    }
}