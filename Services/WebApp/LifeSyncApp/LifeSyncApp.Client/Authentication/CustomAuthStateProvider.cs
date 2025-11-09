using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LifeSyncApp.Client.Authentication
{
    public class CustomAuthStateProvider(IJSRuntime js) : AuthenticationStateProvider
    {
        private const string TokenKey = "authToken";
        private readonly IJSRuntime _js = js;
        private readonly JwtSecurityTokenHandler _handler = new();

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var principal = await BuildPrincipalFromStoredTokenAsync();
            return new AuthenticationState(principal);
        }

        public async Task MarkUserAsAuthenticatedAsync(string token)
        {
            await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
            var principal = CreatePrincipal(token);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        }

        public async Task MarkUserAsLoggedOutAsync()
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
        }

        public async Task<string?> GetTokenAsync()
        {
            var token = await _js.InvokeAsync<string>("localStorage.getItem", TokenKey);
            if (string.IsNullOrWhiteSpace(token)) return null;

            try
            {
                var jwt = _handler.ReadJwtToken(token);
                var exp = jwt.Payload.Exp;
                if (exp.HasValue && DateTimeOffset.FromUnixTimeSeconds(exp.Value) <= DateTimeOffset.UtcNow)
                {
                    await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
                    return null;
                }
                return token;
            }
            catch
            {
                await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
                return null;
            }
        }

        private async Task<ClaimsPrincipal> BuildPrincipalFromStoredTokenAsync()
        {
            var token = await GetTokenAsync();
            return string.IsNullOrWhiteSpace(token)
                ? new ClaimsPrincipal(new ClaimsIdentity())
                : CreatePrincipal(token);
        }

        private ClaimsPrincipal CreatePrincipal(string token)
        {
            var jwt = _handler.ReadJwtToken(token);
            var identity = new CustomClaimsIdentity(jwt.Claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
            return new ClaimsPrincipal(identity);
        }
    }
}
