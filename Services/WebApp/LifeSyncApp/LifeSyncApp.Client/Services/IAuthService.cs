using LifeSyncApp.Client.Models;

namespace LifeSyncApp.Client.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResult>> LoginAsync(LoginRequest request);
        Task<ApiResponse<AuthResult>> RegisterAsync(RegisterRequest request);
        Task LogoutAsync();
        Task<string?> GetTokenAsync();
        Task<UserDTO?> GetCurrentUserAsync();
        Task<bool> IsAuthenticatedAsync();
    }
}
