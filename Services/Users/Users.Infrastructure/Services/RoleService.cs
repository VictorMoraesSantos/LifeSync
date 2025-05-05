using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Security.Claims;
using Users.Application.Interfaces;
using Users.Domain.Entities;

namespace Users.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public RoleService(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<bool> AssignUserToRolesAsync(string userId, IList<string> roles)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            IdentityResult result = await _userManager.AddToRolesAsync(user, roles);
            return result.Succeeded;
        }

        public Task<bool> CreateRoleAsync(string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteRoleAsync(string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EditRoleAsync(string roleName, string newRoleName)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<string>> GetCurrentUserRolesAsync(ClaimsPrincipal userPrincipal)
        {
            User user = await _userManager.GetUserAsync(userPrincipal);
            if (user == null) return new List<string>();

            IList<string> roles = await _userManager.GetRolesAsync(user);

            return roles;
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();

            IList<string> roles = await _userManager.GetRolesAsync(user);

            return roles;
        }

        public Task<bool> RemoveUserFromRolesAsync(string userId, IList<string> roles)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateUserRolesAsync(string userId, IList<string> roles)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            IList<string> currentRoles = await _userManager.GetRolesAsync(user);
            IdentityResult removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded) return false;

            IdentityResult result = await _userManager.AddToRolesAsync(user, roles);
            return result.Succeeded;
        }
    }
}
