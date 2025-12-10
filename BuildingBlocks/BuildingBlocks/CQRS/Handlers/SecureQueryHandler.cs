using BuildingBlocks.Authorization;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BuildingBlocks.CQRS.Handlers
{
    public abstract class SecureQueryHandlerBase
    {
        protected readonly IHttpContextAccessor HttpContext;

        protected SecureQueryHandlerBase(IHttpContextAccessor httpContext)
        {
            HttpContext = httpContext;
        }

        protected ClaimsPrincipal User => HttpContext.HttpContext?.User
            ?? throw new UnauthorizedAccessException("User not authenticated");

        protected Result<T> ValidateAccess<T>(int resourceUserId)
        {
            if (!User.CanAccess(resourceUserId))
            {
                return Result.Failure<T>(
                    Error.Failure("You do not have permission to access this resource")
                );
            }
            return Result.Success(default(T)!);
        }
    }
}