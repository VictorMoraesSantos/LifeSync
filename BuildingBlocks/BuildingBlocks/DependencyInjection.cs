using BuildingBlocks.CQRS.Extensions;
using BuildingBlocks.Validation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBuildingBlocks(this IServiceCollection services, Assembly assembly)
        {

            services.AddValidationService(assembly);
            services.AddMediatorService(assembly);

            return services;
        }
    }
}
