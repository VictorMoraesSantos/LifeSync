using BuildingBlocks.CQRS.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Users.Application
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
