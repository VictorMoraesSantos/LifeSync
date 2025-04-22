using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Users.Application.DTOs.Role;
using Users.Application.DTOs.User;
using Users.Application.Interfaces;
using Users.Domain.Entities;

namespace Users.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public IdentityService(
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        // User section
        public async Task<string> CreateUserAsync(string userName, string password, string email, string firstName, string lastName, IList<string> roles)
        {
            var user = new User(firstName, lastName, email, userName);

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new IdentityException(result.Errors);

            if (roles != null && roles.Any())
            {
                var roleResult = await _userManager.AddToRolesAsync(user, roles);
                if (!roleResult.Succeeded)
                    throw new IdentityException(roleResult.Errors);
            }

            return user.Id.ToString();
        }

        public async Task<bool> SignInUserAsync(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return false;

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (result.Succeeded)
                user.UpdateLastLogin();

            return result.Succeeded;
        }

        public async Task<string> GetUserIdAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user?.Id.ToString();
        }

        public async Task<UserDetailsDTO> GetUserDetailsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new UserDetailsDTO
            {
                Id = user.Id.ToString(),
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles
            };
        }

        public async Task<UserDetailsDTO> GetUserDetailsByUserNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new UserDetailsDTO
            {
                Id = user.Id.ToString(),
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles
            };
        }

        public async Task<string> GetUserNameAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.UserName;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> IsUserNameUniqueAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user == null;
        }

        public async Task<IList<UserSummaryDTO>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.AsNoTracking().ToListAsync();
            return users.Select(u => new UserSummaryDTO
            {
                Id = u.Id.ToString(),
                FullName = u.FullName,
                UserName = u.UserName,
                Email = u.Email
            }).ToList();
        }

        public async Task<IList<UserDetailsDTO>> GetAllUsersDetailsAsync()
        {
            var users = await _userManager.Users.AsNoTracking().ToListAsync();
            var tasks = users.Select(async user =>
            {
                var roles = await _userManager.GetRolesAsync(user);
                return new UserDetailsDTO
                {
                    Id = user.Id.ToString(),
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles
                };
            });
            return await Task.WhenAll(tasks);
        }

        public async Task<bool> UpdateUserProfileAsync(string userId, string firstName, string lastName, string email, IList<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.UpdateProfile(firstName, lastName, email, user.DocumentNumber, user.BirthDate);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded) return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded) return false;

            var addResult = await _userManager.AddToRolesAsync(user, roles);
            return addResult.Succeeded;
        }

        // Role section
        public async Task<bool> CreateRoleAsync(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
                return false;

            var result = await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
            return result.Succeeded;
        }

        public async Task<bool> DeleteRoleAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return false;

            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded;
        }

        public async Task<IList<RoleDTO>> GetRolesAsync()
        {
            var roles = await _roleManager.Roles.AsNoTracking().ToListAsync();
            return roles.Select(r => new RoleDTO
            {
                Id = r.Id.ToString(),
                RoleName = r.Name
            }).ToList();
        }

        public async Task<RoleDTO> GetRoleByIdAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return null;

            return new RoleDTO
            {
                Id = role.Id.ToString(),
                RoleName = role.Name
            };
        }

        public async Task<bool> UpdateRoleAsync(string roleId, string roleName)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return false;

            role.Name = roleName;
            var result = await _roleManager.UpdateAsync(role);
            return result.Succeeded;
        }

        // User's Role section
        public async Task<bool> IsUserInRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();

            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> AssignUserToRolesAsync(string userName, IList<string> roles)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return false;

            var result = await _userManager.AddToRolesAsync(user, roles);
            return result.Succeeded;
        }

        public async Task<bool> UpdateUserRolesAsync(string userName, IList<string> roles)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded) return false;

            var addResult = await _userManager.AddToRolesAsync(user, roles);
            return addResult.Succeeded;
        }
    }

    // Exemplo de exceção customizada para Identity
    public class IdentityException : Exception
    {
        public IEnumerable<IdentityError> Errors { get; }
        public IdentityException(IEnumerable<IdentityError> errors)
            : base(string.Join("; ", errors.Select(e => e.Description)))
        {
            Errors = errors;
        }
    }
}