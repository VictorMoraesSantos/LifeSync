using BuildingBlocks.CQRS.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace TaskManager.Application
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
