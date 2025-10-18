using BuildingBlocks.Results;
using System.Security.Claims;

namespace Users.Application.Interfaces
{
    public interface IRoleService
    {
        Task<Result<IList<string>>> GetUserRolesAsync(string userId);
        Task<Result<IList<string>>> GetCurrentUserRolesAsync(ClaimsPrincipal user);
        Task<Result<bool>> CreateRoleAsync(string roleName);
        Task<Result<bool>> EditRoleAsync(string roleName, string newRoleName);
        Task<Result<bool>> DeleteRoleAsync(string roleName);
        Task<Result<bool>> AssignUserToRolesAsync(string userId, IList<string> roles);
        Task<Result<bool>> RemoveUserFromRolesAsync(string userId, IList<string> roles);
        Task<Result<bool>> UpdateUserRolesAsync(string userId, IList<string> roles);
    }
}
