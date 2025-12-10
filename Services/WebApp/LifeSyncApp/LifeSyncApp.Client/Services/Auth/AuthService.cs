using LifeSyncApp.Client.Authentication;
using LifeSyncApp.Client.Models;
using LifeSyncApp.Client.Services.Contracts;
using System.Net.Http.Json;
using System.Text.Json;

namespace LifeSyncApp.Client.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IApiClient _apiClient;
        private readonly CustomAuthStateProvider _authProvider;
        private const string TokenKey = "authToken";
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public AuthService(IApiClient apiClient, CustomAuthStateProvider authProvider)
        {
            _apiClient = apiClient;
            _authProvider = authProvider;
        }

        private static string Route(string path) => path.StartsWith('/') ? path : $"/{path}";

        private static bool IsJson(HttpResponseMessage response)
        {
            if (response.Content?.Headers?.ContentType == null) return false;
            var mediaType = response.Content.Headers.ContentType.MediaType ?? string.Empty;
            return mediaType.Contains("json", StringComparison.OrdinalIgnoreCase) || mediaType == "application/problem+json";
        }

        private static async Task<(ApiResponse<T>? Parsed, string? Raw)> TryReadApiResponse<T>(HttpResponseMessage response)
        {
            if (response.Content == null)
                return (null, null);

            var raw = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(raw))
                return (null, null);

            if (!IsJson(response))
                return (null, raw);

            try
            {
                return (JsonSerializer.Deserialize<ApiResponse<T>>(raw, JsonOptions), raw);
            }
            catch
            {
                return (null, raw);
            }
        }

        public async Task<ApiResponse<AuthResult>> LoginAsync(LoginRequest request)
        {
            try
            {
                var url = Route("users-service/auth/login");
                var http = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = JsonContent.Create(request)
                };
                var response = await _apiClient.SendAsync(http);

                var (parsed, raw) = await TryReadApiResponse<AuthResult>(response);
                if (parsed?.Success == true && parsed.Data != null)
                {
                    await _authProvider.MarkUserAsAuthenticatedAsync(parsed.Data.AccessToken);
                    return parsed;
                }

                var status = (int)response.StatusCode;
                var errors = parsed?.Errors ?? Array.Empty<string>();
                var msg = errors.Length > 0
                    ? errors
                    : new[] { $"Resposta inesperada do servidor (status {status}). Conteúdo: {(string.IsNullOrWhiteSpace(raw) ? "<vazio>" : raw[..Math.Min(raw.Length, 200)])}" };

                return new ApiResponse<AuthResult> { Success = false, StatusCode = status, Errors = msg };
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthResult> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<AuthResult>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var url = Route("users-service/auth/register");
                var http = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = JsonContent.Create(request)
                };
                var response = await _apiClient.SendAsync(http);

                var (parsed, raw) = await TryReadApiResponse<AuthResult>(response);
                if (parsed?.Success == true && parsed.Data != null)
                {
                    await _authProvider.MarkUserAsAuthenticatedAsync(parsed.Data.AccessToken);
                    return parsed;
                }

                var status = (int)response.StatusCode;
                var errors = parsed?.Errors ?? Array.Empty<string>();
                var msg = errors.Length > 0
                    ? errors
                    : new[] { $"Resposta inesperada do servidor (status {status}). Conteúdo: {(string.IsNullOrWhiteSpace(raw) ? "<vazio>" : raw[..Math.Min(raw.Length, 200)])}" };

                return new ApiResponse<AuthResult> { Success = false, StatusCode = status, Errors = msg };
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthResult> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                var url = Route("users-service/auth/logout");
                await _apiClient.PostAsync<object, object>(url, new { });
            }
            catch { }

            await _authProvider.MarkUserAsLoggedOutAsync();
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _authProvider.GetTokenAsync();
        }

        public Task<UserDTO?> GetCurrentUserAsync()
        {
            // Not used anymore; data comes from claims
            return Task.FromResult<UserDTO?>(null);
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }
    }
}
