using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using BuildingBlocks.CQRS.Extensions;

namespace Financial.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatorService();
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            return services;
        }
    }
}
