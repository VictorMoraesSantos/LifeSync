using BuildingBlocks.Authorization;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BuildingBlocks.CQRS.Handlers
{
    public abstract class SecureCommandHandlerBase
    {
        protected readonly IHttpContextAccessor HttpContext;

        protected SecureCommandHandlerBase(IHttpContextAccessor httpContext)
        {
            HttpContext = httpContext;
        }

        protected ClaimsPrincipal User => HttpContext.HttpContext?.User ?? throw new UnauthorizedAccessException("User not authenticated");

        protected Result<T> ValidateAccess<T>(int resourceUserId)
        {
            if (!User.CanAccess(resourceUserId))
                return Result.Failure<T>(Error.Failure("You do not have permission to access this resource"));

            return Result.Success(default(T)!);
        }

        protected Result ValidateOwnership(int targetUserId)
        {
            if (!User.CanAccess(targetUserId))
                return Result.Failure(Error.Failure("You can only create/modify resources for yourself"));

            return Result.Success();
        }

        protected int GetAuthenticatedUserId()
        {
            var userId = User.GetUserId();
            if (!userId.HasValue)
                throw new UnauthorizedAccessException("User ID not found in token");

            return userId.Value;
        }

        protected bool IsAdmin()
        {
            return User.IsAdmin();
        }
    }
}
