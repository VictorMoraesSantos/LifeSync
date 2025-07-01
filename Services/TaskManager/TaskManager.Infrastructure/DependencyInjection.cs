using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using TaskManager.Application.BackgroundServices;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Repositories;
using TaskManager.Infrastructure.Persistence.Data;
using TaskManager.Infrastructure.Persistence.Repositories;
using TaskManager.Infrastructure.Services;

namespace TaskManager.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext(configuration);
            services.AddMessaging(configuration);

            services.AddScoped<ITaskItemService, TaskItemService>();
            services.AddScoped<ITaskLabelService, TaskLabelService>();

            services.AddHostedService<MigrationHostedService>();
            services.AddHostedService<DueDateReminderService>();

            return services;
        }

        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("Database")!;

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<ITaskItemRepository, TaskItemRepository>();
            services.AddScoped<ITaskLabelRepository, TaskLabelRepository>();

            return services;
        }

        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitCfg = configuration.GetSection("RabbitMQSettings").Get<RabbitMqSettings>();
            services.AddSingleton<IConnectionFactory>(sp =>
                new ConnectionFactory
                {
                    HostName = rabbitCfg.Host,
                    UserName = rabbitCfg.User,
                    Password = rabbitCfg.Password,
                    Port = rabbitCfg.Port,
                    DispatchConsumersAsync = false
                });
            services.AddSingleton<PersistentConnection>();
            services.AddSingleton<IEventBus, EventBus>();

            return services;
        }
    }
}