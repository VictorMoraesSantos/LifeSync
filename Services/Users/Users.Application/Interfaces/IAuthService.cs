using System.Security.Claims;
using Users.Application.DTOs.Auth;
using Users.Application.DTOs.User;

namespace Users.Application.Interfaces
{
    public interface IAuthService
    {
        // Autenticação
        Task<UserDTO> SignInAsync(string email, string password);
        Task<UserDTO> SignUpAsync(string firstName, string lastName, string email, string password);
        Task SignOutAsync(ClaimsPrincipal user);
        Task<bool> UpdateRefreshTokenAsync(string userId, string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);

        // Confirmação de e-mail
        Task<bool> SendEmailConfirmationAsync(string email);
        Task<bool> ConfirmEmailAsync(string userId, string token);

        // Esqueci/Resetar senha
        Task<bool> SendPasswordResetAsync(string email);
        Task<bool> ResetPasswordAsync(string userId, string token, string newPassword);
    }
}

