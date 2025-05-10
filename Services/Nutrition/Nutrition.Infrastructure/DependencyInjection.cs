using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nutrition.Application.Interfaces;
using Nutrition.Domain.Repositories;
using Nutrition.Infrastructure.Data;
using Nutrition.Infrastructure.Repositories;
using Nutrition.Infrastructure.Services;

namespace Nutrition.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("Database")!;

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<IDiaryRepository, DiaryRepository>();
            services.AddScoped<IMealRepository, MealRepository>();
            services.AddScoped<IMealFoodRepository, MealFoodRepository>();
            services.AddScoped<ILiquidRepository, LiquidRepository>();
            services.AddScoped<IDailyProgressRepository, DailyProgressRepository>();

            services.AddScoped<IDiaryService, DiaryService>();
            services.AddScoped<IMealService, MealService>();
            services.AddScoped<IMealFoodService, MealFoodService>();

            return services;
        }
    }
}
