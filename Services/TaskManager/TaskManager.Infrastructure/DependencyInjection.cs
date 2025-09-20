using BuildingBlocks.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddServices();

            services.AddHostedService<MigrationHostedService>();
            services.AddHostedService<DueDateReminderService>();

            return services;
        }

        private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("Database")!;

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<ITaskItemRepository, TaskItemRepository>();
            services.AddScoped<ITaskLabelRepository, TaskLabelRepository>();

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ITaskItemService, TaskItemService>();
            services.AddScoped<ITaskLabelService, TaskLabelService>();

            return services;
        }
    }
}