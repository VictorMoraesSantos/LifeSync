using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.RabbitMQ;
using EmailSender.Application.Contracts;
using EmailSender.Domain.Events;
using EmailSender.Infrastructure.Messaging;
using EmailSender.Infrastructure.Services;
using EmailSender.Infrastructure.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Domain.Repositories;
using Notification.Infrastructure.Persistence.Data;
using Notification.Infrastructure.Persistence.Repositories;
using RabbitMQ.Client;

namespace EmailSender.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext(configuration);
            services.AddEmailSenderInfrastructure(configuration);
            services.AddMessaging(configuration);
            services.AddEventConsumers();
            return services;
        }

        private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("Database")!;

            services.AddDbContext<ApplicationDbContext>(options =>
                 options.UseNpgsql(connectionString));

            services.AddHostedService<MigrationHostedService>();

            services.AddScoped<IEmailMessageRepository, EmailMessageRepository>();

            return services;
        }

        private static IServiceCollection AddEmailSenderInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
            services.AddScoped<IEmailSender, SmtpEmailSender>();
            return services;
        }

        private static IServiceCollection AddEventConsumers(this IServiceCollection services)
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