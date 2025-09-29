using BuildingBlocks.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace BuildingBlocks.Validation.Middleware
{
    public class ValidationExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    title = "Validation Failed",
                    status = (int)HttpStatusCode.BadRequest,
                    errors = ex.Errors
                        .GroupBy(error => error.PropertyName)
                        .ToDictionary(
                            group => group.Key,
                            group => group.Select(error => error.ErrorMessage).ToArray())
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}