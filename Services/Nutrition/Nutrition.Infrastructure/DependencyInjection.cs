using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nutrition.Domain.Repositories;
using Nutrition.Infrastructure.Data;
using Nutrition.Infrastructure.Repositories;

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
            services.AddScoped<ILiquidRepository, LiquidRepository>();
            services.AddScoped<IMealRepository, MealRepository>();
            services.AddScoped<IMealFoodRepository, MealFoodRepository>();
            services.AddScoped<IDailyProgressRepository, DailyProgressRepository>();

            return services;
        }
    }
}
