using BuildingBlocks.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> SignInManager)
        {
            _userManager = userManager;
            _signInManager = SignInManager;
        }

        public async Task<UserDTO> LoginAsync(string email, string password)
        {
            User user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new BadRequestException("Invalid credentials");

            SignInResult result = await _signInManager.PasswordSignInAsync(user, password, false, false);
            if (!result.Succeeded)
                throw new BadRequestException("Invalid credentials");

            IList<string> roles = await _userManager.GetRolesAsync(user);
            UserDTO userDTO = UserMapper.ToDto(user);
            userDTO = userDTO with { Roles = roles.ToList() };

            return userDTO;
        }

        public async Task<UserDTO> SignUpAsync(string firstName, string lastName, string email, string password)
        {
            Name name = new Name(firstName, lastName);
            Contact contact = new Contact(email);

            User user = new User(name, contact);

            IdentityResult result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new BadRequestException("Invalid credentials", result.Errors.Select(e => e.Description));

            User createdUser = await _userManager.FindByEmailAsync(email);
            IList<string> roles = await _userManager.GetRolesAsync(createdUser);

            UserDTO userDTO = UserMapper.ToDto(createdUser);
            userDTO = userDTO with { Roles = roles.ToList() };

            return userDTO;
        }

        public async Task LogoutAsync(ClaimsPrincipal user)
        {
            await _signInManager.SignOutAsync();

            User currentUser = await _userManager.GetUserAsync(user);
            currentUser.RefreshToken = null;
            currentUser.RefreshTokenExpiryTime = DateTime.MinValue;

            IdentityResult result = await _userManager.UpdateAsync(currentUser);
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }

        public async Task<bool> UpdateRefreshTokenAsync(string userId, string refreshToken)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return false;

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            IdentityResult result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            IdentityResult result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if(!result.Succeeded)
                throw new BadRequestException("Invalid token", result.Errors.Select(e => e.Description));

            return result.Succeeded;
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            User user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user == null) return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.MinValue;

            IdentityResult result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<string> SendEmailConfirmationAsync(string email)
        {
            User user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new BadRequestException("Invalid request");

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            return token;
        }

        public async Task<string> SendPasswordResetAsync(string email)
        {
            User user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new BadRequestException("Invalid request");

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return token;
        }

        public async Task<bool> ChangePasswordAsync(ClaimsPrincipal user, string currentPassword, string newPassword)
        {
            User currentUser = await _userManager.GetUserAsync(user);
            if (currentUser == null || !user.Identity.IsAuthenticated)
                throw new BadRequestException("InvalidRequest");

            IdentityResult result = await _userManager.ChangePasswordAsync(currentUser, currentPassword, newPassword);
            return result.Succeeded;
        }
    }
}