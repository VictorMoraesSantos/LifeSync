using LifeSyncApp.DTOs.Auth;

namespace LifeSyncApp.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(LoginRequest request);
        Task<AuthResult> RegisterAsync(RegisterRequest request);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetAccessTokenAsync();
        Task<int> GetUserIdAsync();
    }
}
