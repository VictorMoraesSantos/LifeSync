using System.Security.Claims;

namespace BuildingBlocks.Authorization
{
    public static class AuthorizationExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("UserId") ?? user.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(claim?.Value, out var id) ? id : null;
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.IsInRole("Admin") || user.IsInRole("Administrator");
        }

        public static bool CanAccess(this ClaimsPrincipal user, int resourceUserId)
        {
            if (user.IsAdmin()) return true;
            
            var userId = user.GetUserId();
            return userId.HasValue && userId.Value == resourceUserId;
        }
    }
}