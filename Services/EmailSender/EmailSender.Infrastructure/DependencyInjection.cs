using BuildingBlocks.CQRS.Publisher;
using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.RabbitMQ;
using EmailSender.Application.Contracts;
using EmailSender.Infrastructure.Messaging;
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
        public static IServiceCollection AddEmailSenderInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var smtpSettings = configuration.GetSection("SmtpSettings").Get<SmtpSettings>();

            services.AddSingleton(sp =>
            {
                var client = new SmtpClient(smtpSettings.Host, smtpSettings.Port)
                {
                    Credentials = new System.Net.NetworkCredential(smtpSettings.User, smtpSettings.Password),
                    EnableSsl = smtpSettings.EnableSsl
                };
                return client;
            });

            services.AddSingleton(sp =>
            {
                var factory = new ConnectionFactory()
                {
                    HostName = configuration.GetValue<string>("RabbitMQ:Host"),
                    UserName = configuration.GetValue<string>("RabbitMQ:User"),
                    Password = configuration.GetValue<string>("RabbitMQ:Password"),
                    DispatchConsumersAsync = true
                };
                return new PersistentConnection(factory);
            });

            services.AddSingleton<PersistentConnection>();
            services.AddSingleton<IEventConsumer, EventConsumer>();
            services.AddScoped<IEmailSender, SmtpEmailSender>();
            services.AddSingleton<IPublisher, Publisher>();
            services.AddSingleton<RabbitMqEventConsumer>();
            return services;
        }
    }
}