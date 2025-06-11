using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.RabbitMQ;
using EmailSender.Application.Contracts;
using EmailSender.Infrastructure.Extensions;
using EmailSender.Infrastructure.Messaging;
using EmailSender.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            services.AddScoped<IEmailSender, SmtpEmailSender>();
            services.AddSingleton<RabbitMqEventConsumer>();

            return services;
        }
    }
}