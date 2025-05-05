using System.Security.Claims;

namespace Users.Application.Interfaces
{
    public interface IRoleService
    {
        Task<IList<string>> GetUserRolesAsync(string userId);
        Task<IList<string>> GetCurrentUserRolesAsync(ClaimsPrincipal user);
        Task<bool> CreateRoleAsync(string roleName);
        Task<bool> EditRoleAsync(string roleName, string newRoleName);
        Task<bool> DeleteRoleAsync(string roleName);
        Task<bool> AssignUserToRolesAsync(string userId, IList<string> roles);
        Task<bool> RemoveUserFromRolesAsync(string userId, IList<string> roles);
        Task<bool> UpdateUserRolesAsync(string userId, IList<string> roles);
    }
}
