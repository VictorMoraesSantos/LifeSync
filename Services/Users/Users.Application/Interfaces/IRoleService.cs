using BuildingBlocks.Results;
using System.Security.Claims;

namespace Users.Application.Interfaces
{
    public interface IRoleService
    {
        Task<Result<IEnumerable<string>>> GetUserRolesAsync(string userId);
        Task<Result<IEnumerable<string>>> GetCurrentUserRolesAsync(ClaimsPrincipal user);
        Task<Result<bool>> CreateRoleAsync(string roleName);
        Task<Result<bool>> EditRoleAsync(string roleName, string newRoleName);
        Task<Result<bool>> DeleteRoleAsync(string roleName);
        Task<Result<bool>> AssignUserToRolesAsync(string userId, IEnumerable<string> roles);
        Task<Result<bool>> RemoveUserFromRolesAsync(string userId, IEnumerable<string> roles);
        Task<Result<bool>> UpdateUserRolesAsync(string userId, IEnumerable<string> roles);
    }
}
