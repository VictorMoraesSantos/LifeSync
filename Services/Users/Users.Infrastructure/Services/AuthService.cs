using BuildingBlocks.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Users.Application.DTOs.User;
using Users.Application.Interfaces;
using Users.Application.Mapping;
using Users.Domain.Entities;
using Users.Domain.ValueObjects;

namespace Users.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> SignInManager,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = SignInManager;
            _logger = logger;
        }

        public async Task<Result<UserDTO>> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return Result<UserDTO>.Failure(Error.Problem("Invalid credentials"));

                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (!result.Succeeded)
                    return Result<UserDTO>.Failure(Error.Problem("Invalid credentials"));

                var roles = await _userManager.GetRolesAsync(user);
                var dto = UserMapper.ToDto(user) with { Roles = roles.ToList() };

                return Result<UserDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while logging in user {Email}", email);
                return Result<UserDTO>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<UserDTO>> SignUpAsync(string firstName, string lastName, string email, string password)
        {
            try
            {
                var name = new Name(firstName, lastName);
                var contact = new Contact(email);
                var user = new User(name, contact);
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                    return Result<UserDTO>.Failure(Error.Problem(string.Join("; ", result.Errors.Select(e => e.Description))));

                var createdUser = await _userManager.FindByEmailAsync(email);
                var roles = await _userManager.GetRolesAsync(createdUser);
                var dto = UserMapper.ToDto(createdUser) with { Roles = roles.ToList() };

                _logger.LogInformation("User created successfully: {Email}", email);
                return Result<UserDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while signing up user {Email}", email);
                return Result<UserDTO>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result> LogoutAsync(ClaimsPrincipal user)
        {
            try
            {
                await _signInManager.SignOutAsync();

                var currentUser = await _userManager.GetUserAsync(user);
                if (currentUser == null)
                    return Result.Failure(Error.Failure("Invalid request"));

                currentUser.RefreshToken = null;
                currentUser.RefreshTokenExpiryTime = DateTime.MinValue;
                await _userManager.UpdateAsync(currentUser);

                _logger.LogInformation("User logged out successfully");
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while logging out");
                return Result.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> ConfirmEmailAsync(string userId, string token)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Result<bool>.Failure(Error.NotFound("User not found"));

                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                    return Result<bool>.Failure(Error.Problem(string.Join("; ", result.Errors.Select(e => e.Description))));

                _logger.LogInformation("Email confirmed for user {UserId}", userId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while confirming email for user {UserId}", userId);
                return Result<bool>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateRefreshTokenAsync(string userId, string refreshToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                    return Result<bool>.Failure(Error.Problem("Invalid or expired refresh token"));

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    return Result<bool>.Failure(Error.Problem(string.Join("; ", result.Errors.Select(e => e.Description))));

                _logger.LogInformation("Refresh token updated for user {UserId}", userId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating refresh token for user {UserId}", userId);
                return Result<bool>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Result<bool>.Failure(Error.NotFound("User not found"));

                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
                if (!result.Succeeded)
                    return Result<bool>.Failure(Error.Problem(string.Join("; ", result.Errors.Select(e => e.Description))));

                _logger.LogInformation("Password reset for user {UserId}", userId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while resetting password for user {UserId}", userId);
                return Result<bool>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> RevokeRefreshTokenAsync(string refreshToken)
        {
            try
            {
                var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);
                if (user == null)
                    return Result<bool>.Failure(Error.NotFound("User not found"));

                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = DateTime.MinValue;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    return Result<bool>.Failure(Error.Problem(string.Join("; ", result.Errors.Select(e => e.Description))));

                _logger.LogInformation("Refresh token revoked for user {UserId}", user.Id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while revoking refresh token");
                return Result<bool>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<string>> SendEmailConfirmationAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return Result<string>.Failure(Error.NotFound("User not found"));

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                _logger.LogInformation("Email confirmation token generated for {Email}", email);

                return Result<string>.Success(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while generating email confirmation token for {Email}", email);
                return Result<string>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<string>> SendPasswordResetAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return Result<string>.Failure(Error.NotFound("User not found"));

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                _logger.LogInformation("Password reset token generated for {Email}", email);

                return Result<string>.Success(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while generating password reset token for {Email}", email);
                return Result<string>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> ChangePasswordAsync(ClaimsPrincipal user, string currentPassword, string newPassword)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(user);
                if (currentUser == null || !(user.Identity?.IsAuthenticated ?? false))
                    return Result<bool>.Failure(Error.Problem("Invalid request"));

                var result = await _userManager.ChangePasswordAsync(currentUser, currentPassword, newPassword);
                if (!result.Succeeded)
                    return Result<bool>.Failure(Error.Problem(string.Join("; ", result.Errors.Select(e => e.Description))));

                _logger.LogInformation("Password changed for user {UserId}", currentUser.Id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while changing password");
                return Result<bool>.Failure(Error.Failure(ex.Message));
            }
        }
    }
}