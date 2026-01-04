using BuildingBlocks.Results;
using System.Security.Claims;
using Users.Application.DTOs.User;

namespace Users.Application.Interfaces
{
    public interface IAuthService
    {
        // Autenticação
        Task<Result<UserDTO>> LoginAsync(string email, string password);
        Task<Result<UserDTO>> SignUpAsync(string firstName, string lastName, string email, string password);
        Task<Result> LogoutAsync(ClaimsPrincipal user);
        Task<Result<bool>> UpdateRefreshTokenAsync(string userId, string refreshToken);
        Task<Result<bool>> RevokeRefreshTokenAsync(string refreshToken);

        // Confirmação de e-mail
        Task<Result<string>> SendEmailConfirmationAsync(string email);
        Task<Result<bool>> ConfirmEmailAsync(string userId, string token);

        // Esqueci/Resetar senha
        Task<Result<string>> SendPasswordResetAsync(string email);
        Task<Result<bool>> ResetPasswordAsync(string userId, string token, string newPassword);
        Task<Result<bool>> ChangePasswordAsync(ClaimsPrincipal user, string currentPassword, string newPassword);
    }
}

