using BuildingBlocks;
using EmailSender.Application.Features;
using Microsoft.Extensions.DependencyInjection;
using Notification.Application.Contracts;
using Notification.Application.Factories;
using Notification.Application.Strategies;
using System.Reflection;

namespace EmailSender.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddBuildingBlocks(Assembly.GetExecutingAssembly());

            services.AddScoped<IProcessEmailEventUseCase, ProcessEmailEventUseCase>();
            services.AddScoped<IEmailEventStrategy, UserRegisteredEmailStrategy>();
            services.AddScoped<IEmailEventStrategy, OrderPlacedEmailStrategy>();
            services.AddScoped<IEmailEventStrategyResolver, EmailEventStrategyResolver>();

            return services;
        }
    }
}
