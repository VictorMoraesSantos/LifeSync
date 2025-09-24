using BuildingBlocks;
using Microsoft.Extensions.DependencyInjection;

namespace TaskManager.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddBuildingBlocks();

            return services;
        }
    }
}
