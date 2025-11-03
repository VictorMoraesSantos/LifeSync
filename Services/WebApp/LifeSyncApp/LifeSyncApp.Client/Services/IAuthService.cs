using LifeSyncApp.Client.Models.Auth;
using LifeSyncApp.Client.Models.Common;

namespace LifeSyncApp.Client.Services
{
    public interface IAuthService
    {
        Task<HttpResult<AuthResult>> LoginAsync(LoginRequest request);
        Task<HttpResult<AuthResult>> RegisterAsync(RegisterRequest request);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetTokenAsync();
        Task<UserDto?> GetCurrentUserAsync();
        Task SetTokenAsync(string token);
        Task SetCurrentUserAsync(UserDto user);
    }
}
