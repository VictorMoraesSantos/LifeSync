using BuildingBlocks.Validation.Middleware;
using Microsoft.AspNetCore.Builder;

namespace BuildingBlocks.Validation.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseValidationExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ValidationExceptionHandlingMiddleware>();
        }
    }
}