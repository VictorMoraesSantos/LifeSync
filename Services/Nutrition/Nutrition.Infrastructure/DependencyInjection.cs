using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nutrition.Application.Interfaces;
using Nutrition.Domain.Repositories;
using Nutrition.Infrastructure.DataSeeders;
using Nutrition.Infrastructure.Persistence.Data;
using Nutrition.Infrastructure.Persistence.Repositories;
using Nutrition.Infrastructure.Services;

namespace Nutrition.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext(configuration);
            services.AddServices();
            services.AddSeeder();

            return services;
        }

        private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("Database")!;

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddHostedService<MigrationHostedService>();

            services.AddScoped<IDiaryRepository, DiaryRepository>();
            services.AddScoped<IMealRepository, MealRepository>();
            services.AddScoped<IMealFoodRepository, MealFoodRepository>();
            services.AddScoped<ILiquidRepository, LiquidRepository>();
            services.AddScoped<IDailyProgressRepository, DailyProgressRepository>();

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IDiaryService, DiaryService>();
            services.AddScoped<IMealService, MealService>();
            services.AddScoped<IMealFoodService, MealFoodService>();
            services.AddScoped<ILiquidService, LiquidService>();
            services.AddScoped<IDailyProgressService, DailyProgressService>();

            return services;
        }

        private static IServiceCollection AddSeeder(this IServiceCollection services)
        {
            services.AddScoped<TablesCsvSeeder>();
            services.AddHostedService<SeederHostedService>();
            return services;
        }
    }
}
