using BuildingBlocks.CQRS.Pipeline;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks.Validation
{
    public static class ValidationServiceCollectionExtensions
    {
        public static IServiceCollection AddValidationService(this IServiceCollection services, Assembly assembly)
        {
            services.AddValidatorsFromAssembly(assembly);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
