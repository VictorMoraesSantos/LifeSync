using System.Security.Claims;
using Users.Application.DTOs.User;

namespace Users.Application.Interfaces
{
    public interface IAuthService
    {
        // Autenticação
        Task<UserDTO> LoginAsync(string email, string password);
        Task<UserDTO> SignUpAsync(string firstName, string lastName, string email, string password);
        Task LogoutAsync(ClaimsPrincipal user);
        Task<bool> UpdateRefreshTokenAsync(string userId, string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);

        // Confirmação de e-mail
        Task<string> SendEmailConfirmationAsync(string email);
        Task<bool> ConfirmEmailAsync(string userId, string token);

        // Esqueci/Resetar senha
        Task<string> SendPasswordResetAsync(string email);
        Task<bool> ResetPasswordAsync(string userId, string token, string newPassword);
        Task<bool> ChangePasswordAsync(ClaimsPrincipal user, string currentPassword, string newPassword);
    }
}

