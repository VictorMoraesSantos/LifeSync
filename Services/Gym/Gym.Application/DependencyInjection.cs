using BuildingBlocks;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Gym.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddBuildingBlocks(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
