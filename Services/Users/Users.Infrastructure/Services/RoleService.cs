using BuildingBlocks.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Users.Application.Interfaces;
using Users.Domain.Entities;

namespace Users.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RoleService> _logger;

        public RoleService(
            RoleManager<IdentityRole<int>> roleManager,
            UserManager<User> userManager,
            ILogger<RoleService> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Result<bool>> CreateRoleAsync(string roleName)
        {
            try
            {
                if (await _roleManager.RoleExistsAsync(roleName))
                    return Result.Failure<bool>(Error.Conflict($"Role '{roleName}' already exists"));

                var result = await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
                if (!result.Succeeded)
                    return Result.Failure<bool>(Error.Problem(string.Join("; ", result.Errors.Select(e => e.Description))));

                _logger.LogInformation("Role created successfully: {Role}", roleName);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating role {Role}", roleName);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> EditRoleAsync(string roleName, string newRoleName)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                    return Result.Failure<bool>(Error.NotFound($"Role '{roleName}' not found"));

                role.Name = newRoleName;
                var result = await _roleManager.UpdateAsync(role);
                if (!result.Succeeded)
                    return Result.Failure<bool>(Error.Problem(string.Join("; ", result.Errors.Select(e => e.Description))));

                _logger.LogInformation("Role renamed from {Old} to {New}", roleName, newRoleName);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while editing role {Role}", roleName);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteRoleAsync(string roleName)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                    return Result.Failure<bool>(Error.NotFound($"Role '{roleName}' not found"));

                var result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                    return Result.Failure<bool>(Error.Problem(string.Join("; ", result.Errors.Select(e => e.Description))));

                _logger.LogInformation("Role deleted: {Role}", roleName);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting role {Role}", roleName);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IList<string>>> GetUserRolesAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Result.Failure<IList<string>>(Error.NotFound("User not found"));

                var roles = await _userManager.GetRolesAsync(user);
                return Result.Success(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting roles for user {UserId}", userId);
                return Result.Failure<IList<string>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IList<string>>> GetCurrentUserRolesAsync(ClaimsPrincipal userPrincipal)
        {
            try
            {
                var user = await _userManager.GetUserAsync(userPrincipal);
                if (user == null)
                    return Result.Failure<IList<string>>(Error.NotFound("User not found"));

                var roles = await _userManager.GetRolesAsync(user);
                return Result.Success(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting current user roles");
                return Result.Failure<IList<string>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> AssignUserToRolesAsync(string userId, IList<string> roles)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Result.Failure<bool>(Error.NotFound("User not found"));

                var result = await _userManager.AddToRolesAsync(user, roles);
                if (!result.Succeeded)
                    return Result.Failure<bool>(Error.Problem(string.Join("; ", result.Errors.Select(e => e.Description))));

                _logger.LogInformation("Assigned roles {Roles} to user {UserId}", string.Join(",", roles), userId);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while assigning roles to user {UserId}", userId);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> RemoveUserFromRolesAsync(string userId, IList<string> roles)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Result.Failure<bool>(Error.NotFound("User not found"));

                var result = await _userManager.RemoveFromRolesAsync(user, roles);
                if (!result.Succeeded)
                    return Result.Failure<bool>(Error.Problem(string.Join("; ", result.Errors.Select(e => e.Description))));

                _logger.LogInformation("Removed roles {Roles} from user {UserId}", string.Join(",", roles), userId);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while removing roles from user {UserId}", userId);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateUserRolesAsync(string userId, IList<string> roles)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Result.Failure<bool>(Error.NotFound("User not found"));

                var current = await _userManager.GetRolesAsync(user);
                var rem = await _userManager.RemoveFromRolesAsync(user, current);
                if (!rem.Succeeded)
                    return Result.Failure<bool>(Error.Problem(string.Join("; ", rem.Errors.Select(e => e.Description))));

                if (roles != null && roles.Any())
                {
                    var add = await _userManager.AddToRolesAsync(user, roles);
                    if (!add.Succeeded)
                        return Result.Failure<bool>(Error.Problem(string.Join("; ", add.Errors.Select(e => e.Description))));
                }

                _logger.LogInformation("Updated roles for user {UserId}. Previous: {Prev}. New: {New}",
                    userId, string.Join(",", current), roles == null ? "" : string.Join(",", roles));

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating roles for user {UserId}", userId);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }
    }
}