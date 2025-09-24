using BuildingBlocks.CQRS.Extensions;
using BuildingBlocks.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBuildingBlocks(this IServiceCollection services)
        {

            services.AddValidationService();
            services.AddMediatorService();

            return services;
        }
    }
}
