using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Users.Application.Interfaces;
using Users.Domain.Entities;

namespace Users.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly UserManager<User> _userManager;

        public RoleService(
            RoleManager<IdentityRole<int>> roleManager,
            UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<bool> CreateRoleAsync(string roleName)
        {
            var result = await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
            return result.Succeeded;
        }

        public async Task<bool> EditRoleAsync(string roleName, string newRoleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) return false;
            role.Name = newRoleName;
            var result = await _roleManager.UpdateAsync(role);
            return result.Succeeded;
        }

        public async Task<bool> DeleteRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) return false;
            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded;
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user == null
                ? new List<string>()
                : await _userManager.GetRolesAsync(user);
        }

        public async Task<IList<string>> GetCurrentUserRolesAsync(ClaimsPrincipal userPrincipal)
        {
            var user = await _userManager.GetUserAsync(userPrincipal);
            return user == null
                ? new List<string>()
                : await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> AssignUserToRolesAsync(string userId, IList<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;
            var result = await _userManager.AddToRolesAsync(user, roles);
            return result.Succeeded;
        }

        public async Task<bool> RemoveUserFromRolesAsync(string userId, IList<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            return result.Succeeded;
        }

        public async Task<bool> UpdateUserRolesAsync(string userId, IList<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var current = await _userManager.GetRolesAsync(user);
            var rem = await _userManager.RemoveFromRolesAsync(user, current);
            if (!rem.Succeeded) return false;

            var add = await _userManager.AddToRolesAsync(user, roles);
            return add.Succeeded;
        }
    }
}