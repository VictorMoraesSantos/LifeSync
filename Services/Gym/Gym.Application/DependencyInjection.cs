using BuildingBlocks.CQRS.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Gym.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatorService();

            return services;
        }
    }
}
