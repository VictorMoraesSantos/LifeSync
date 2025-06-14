﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Repositories;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Infrastructure.Services;

namespace TaskManager.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("Database")!;

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<ITaskItemService, TaskItemService>();
            services.AddScoped<ITaskLabelService, TaskLabelService>();

            services.AddScoped<ITaskItemRepository, TaskItemRepository>();
            services.AddScoped<ITaskLabelRepository, TaskLabelRepository>();

            services.AddHostedService<MigrationHostedService>();

            return services;
        }
    }
}
