using Financial.Application.Contracts;
using Financial.Domain.Repositories;
using Financial.Infrastructure.BackgroundServices;
using Financial.Infrastructure.Persistence;
using Financial.Infrastructure.Persistence.Repositories;
using Financial.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Financial.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseNpgsql(configuration.GetConnectionString("Database"))
                    .ConfigureWarnings(w => w.Log(RelationalEventId.PendingModelChangesWarning)));

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IRecurrenceScheduleRepository, RecurrenceScheduleRepository>();

            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IRecurrenceScheduleService, RecurrenceScheduleService>();


            services.AddHostedService<MigrationHostedService>();
            services.AddHostedService<RecurrenceProcessorService>();

            return services;
        }
    }
}
