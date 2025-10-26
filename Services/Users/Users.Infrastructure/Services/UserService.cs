using BuildingBlocks.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Users.Application.DTOs.User;
using Users.Application.Interfaces;
using Users.Application.Mapping;
using Users.Domain.Entities;
using Users.Domain.Errors;
using Users.Domain.ValueObjects;

namespace Users.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            ILogger<UserService> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<UserDTO>> GetCurrentUserDetailsAsync(ClaimsPrincipal user)
        {
            try
            {
                var entity = await _userManager.GetUserAsync(user);
                if (entity is null)
                    return Result.Failure<UserDTO>(UserErrors.NotFound(int.Parse(user.Identity?.ToString()!)));

                var roles = await _userManager.GetRolesAsync(entity);
                var dto = UserMapper.ToDto(entity) with { Roles = roles.ToList() };

                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user details");
                return Result.Failure<UserDTO>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateCurrentUserProfileAsync(ClaimsPrincipal user, string firstName, string lastName, string email)
        {
            try
            {
                var entity = await _userManager.GetUserAsync(user);
                if (entity is null)
                    return Result.Failure<bool>(UserErrors.NotFound(int.Parse(user.Identity?.ToString()!)));

                var name = new Name(firstName, lastName);
                var contact = new Contact(email);

                entity.UpdateProfile(name, contact);

                var result = await _userManager.UpdateAsync(entity);
                if (!result.Succeeded)
                    return Result.Failure<bool>(Error.Problem(string.Join("; ", result.Errors.Select(e => e.Description))));

                _logger.LogInformation("User profile updated for {UserId}", entity.Id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating current user profile");
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> ChangeCurrentUserPasswordAsync(ClaimsPrincipal user, string currentPassword, string newPassword)
        {
            try
            {
                var entity = await _userManager.GetUserAsync(user);
                if (entity is null)
                    return Result.Failure<bool>(Error.NotFound("User not found"));

                var result = await _userManager.ChangePasswordAsync(entity, currentPassword, newPassword);
                if (!result.Succeeded)
                    return Result.Failure<bool>(Error.Problem(string.Join("; ", result.Errors.Select(e => e.Description))));

                _logger.LogInformation("Password changed for user {UserId}", entity.Id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing current user password");
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteCurrentUserAsync(ClaimsPrincipal user)
        {
            try
            {
                var entity = await _userManager.GetUserAsync(user);
                if (entity is null)
                    return Result.Failure<bool>(Error.NotFound("User not found"));

                var result = await _userManager.DeleteAsync(entity);
                if (!result.Succeeded)
                    return Result.Failure<bool>(Error.Problem(string.Join("; ", result.Errors.Select(e => e.Description))));

                _logger.LogInformation("Deleted current user {UserId}", entity.Id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting current user");
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<UserDTO>> GetUserDetailsAsync(string userId)
        {
            try
            {
                var entity = await _userManager.FindByIdAsync(userId);
                if (entity is null)
                    return Result.Failure<UserDTO>(Error.NotFound("User not found"));

                var roles = await _userManager.GetRolesAsync(entity);
                var dto = UserMapper.ToDto(entity) with { Roles = roles.ToList() };

                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user details for {UserId}", userId);
                return Result.Failure<UserDTO>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<UserDTO>>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userManager.Users.AsNoTracking().ToListAsync();
                var dtos = users.Select(UserMapper.ToDto).ToList();

                return Result.Success<IEnumerable<UserDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return Result.Failure<IEnumerable<UserDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<UserDTO>>> GetAllUsersDetailsAsync()
        {
            try
            {
                var users = await _userManager.Users.AsNoTracking().ToListAsync();
                var list = new List<UserDTO>(users.Count);

                foreach (var u in users)
                {
                    var roles = await _userManager.GetRolesAsync(u);
                    list.Add(UserMapper.ToDto(u) with { Roles = roles.ToList() });
                }

                return Result.Success<IEnumerable<UserDTO>>(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users details");
                return Result.Failure<IEnumerable<UserDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> IsUserEmailUniqueAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                return Result.Success(user == null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email uniqueness for {Email}", email);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateUserProfileAsync(UpdateUserDTO dto)
        {
            try
            {
                var entity = await _userManager.FindByIdAsync(dto.Id.ToString());
                if (entity is null)
                    return Result.Failure<bool>(UserErrors.NotFound(dto.Id));

                if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
                    return Result.Failure<bool>(NameErrors.NullName);

                Contact contact;
                try
                {
                    contact = new Contact(dto.Email);
                }
                catch
                {
                    return Result.Failure<bool>(ContactErrors.InvalidFormat);
                }

                var name = new Name(dto.FirstName, dto.LastName);
                entity.UpdateProfile(name, contact);

                var result = await _userManager.UpdateAsync(entity);
                if (!result.Succeeded)
                    return Result.Failure<bool>(Error.Problem(string.Join("; ", result.Errors.Select(e => e.Description))));

                _logger.LogInformation("User profile updated for {UserId}", dto.Id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile for {UserId}", dto.Id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteUserAsync(string userId)
        {
            try
            {
                var entity = await _userManager.FindByIdAsync(userId);
                if (entity is null)
                    return Result.Failure<bool>(Error.NotFound("User not found"));

                var result = await _userManager.DeleteAsync(entity);
                if (!result.Succeeded)
                    return Result.Failure<bool>(Error.Problem(string.Join("; ", result.Errors.Select(e => e.Description))));

                _logger.LogInformation("Deleted user {UserId}", userId);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", userId);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }
    }
}