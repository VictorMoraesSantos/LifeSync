using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.CQRS.Extensions;

namespace Financial.Application
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
