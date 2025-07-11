using Gym.Domain.Repositories;
using Gym.Infrastructure.Persistence.Data;
using Gym.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gym.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext(configuration);
            services.AddServices();

            return services;
        }

        private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("Database")!;

            services.AddDbContext<ApplicationDbContext>(options =>
                 options.UseNpgsql(connectionString));

            services.AddScoped<ICompletedExerciseRepository, CompletedExerciseRepository>();
            services.AddScoped<IExerciseRepository, ExerciseRepository>();
            services.AddScoped<IRoutineRepository, RoutineRepository>();
            services.AddScoped<IRoutineExerciseRepository, RoutineExerciseRepository>();
            services.AddScoped<ITrainingSessionRepository, TrainingSessionRepository>();

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            //services.AddScoped<ICompletedExerciseService, CompletedExerciseService>();
            //services.AddScoped<IExerciseService, ExerciseService>();
            //services.AddScoped<IRoutineService, RoutineService>();
            //services.AddScoped<IRoutineExerciseService, RoutineExerciseService>();
            //services.AddScoped<ITrainingSessionService, TrainingSessionService>();

            return services;
        }
    }
}
