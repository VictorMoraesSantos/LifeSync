using LifeSyncApp.DTOs.Auth;
using LifeSyncApp.DTOs.Common;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Text.Json;

namespace LifeSyncApp.Services.Auth
{
    public class AuthService : IAuthService
    {
        private const string AccessTokenKey = "access_token";
        private const string RefreshTokenKey = "refresh_token";
        private const string UserIdKey = "user_id";
        private const string BaseUrl = "/auth";

        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public AuthService(
            IHttpClientFactory httpClientFactory,
            ILogger<AuthService> logger,
            JsonSerializerOptions jsonOptions)
        {
            _httpClient = httpClientFactory.CreateClient("LifeSyncApi");
            _logger = logger;
            _jsonOptions = jsonOptions;

            // Force WebAuthenticator static initialization before first use.
            // This eliminates the race condition where the first OAuth callback
            // arrives before the static event handlers are registered.
#if ANDROID
            _ = WebAuthenticator.Default;
#endif
        }

        public async Task<AuthResult> LoginAsync(LoginRequest request)
        {
            System.Diagnostics.Debug.WriteLine($"[Auth] POST {BaseUrl}/login - Email: {request.Email}");

            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/login", request, _jsonOptions);
            var body = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"[Auth] Login response {response.StatusCode}: {body}");

            if (!response.IsSuccessStatusCode)
            {
                var friendlyMessage = TryExtractError(body) ?? "E-mail ou senha incorretos.";
                throw new HttpRequestException(friendlyMessage);
            }

            var apiResponse = JsonSerializer.Deserialize<ApiSingleResponse<AuthResult>>(body, _jsonOptions);
            var result = apiResponse?.Data ?? throw new InvalidOperationException($"Resposta inesperada da API: {body}");
            await StoreTokensAsync(result);
            return result;
        }

        public async Task<AuthResult> RegisterAsync(RegisterRequest request)
        {
            System.Diagnostics.Debug.WriteLine($"[Auth] POST {BaseUrl}/register - Email: {request.Email}");

            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/register", request, _jsonOptions);
            var body = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"[Auth] Register response {response.StatusCode}: {body}");

            if (!response.IsSuccessStatusCode)
            {
                var friendlyMessage = TryExtractError(body) ?? $"Erro ao criar conta: {response.ReasonPhrase}";
                throw new HttpRequestException(friendlyMessage);
            }

            var apiResponse = JsonSerializer.Deserialize<ApiSingleResponse<AuthResult>>(body, _jsonOptions);
            var result = apiResponse?.Data ?? throw new InvalidOperationException($"Resposta inesperada da API: {body}");
            await StoreTokensAsync(result);
            return result;
        }

        public async Task<AuthResult> GoogleLoginAsync()
        {
            System.Diagnostics.Debug.WriteLine("[Auth] Starting Google login via backend OAuth flow");

            var baseAddress = _httpClient.BaseAddress?.ToString().TrimEnd('/');
            var state = Guid.NewGuid().ToString("N");

            var authResult = await WebAuthenticator.Default.AuthenticateAsync(
                new WebAuthenticatorOptions
                {
                    Url = new Uri($"{baseAddress}/auth/google-login?state={state}"),
                    CallbackUrl = new Uri("com.lifesync.app://callback")
                });

#if ANDROID
            // After WebAuthenticator completes, the Chrome Custom Tab may still
            // be visible on top. Bring the app back to the foreground. At this
            // point the WebAuthenticatorIntermediateActivity has already finished,
            // so this is safe and will not interfere with the auth flow.
            var context = Android.App.Application.Context;
            var launch = context.PackageManager!.GetLaunchIntentForPackage(context.PackageName!);
            if (launch != null)
            {
                launch.AddFlags(Android.Content.ActivityFlags.NewTask | Android.Content.ActivityFlags.SingleTop);
                context.StartActivity(launch);
            }
#endif

            var error = authResult?.Get("error");
            if (!string.IsNullOrEmpty(error))
                throw new InvalidOperationException($"Erro no login com Google: {Uri.UnescapeDataString(error)}");

            var accessToken = authResult?.Get("access_token");
            var refreshToken = authResult?.Get("refresh_token");
            var userId = authResult?.Get("user_id");

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(userId))
                throw new InvalidOperationException("Resposta incompleta do login com Google.");

            await SecureStorage.SetAsync(AccessTokenKey, accessToken);
            await SecureStorage.SetAsync(RefreshTokenKey, refreshToken);
            await SecureStorage.SetAsync(UserIdKey, userId);

            return new AuthResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new UserDTO { Id = userId }
            };
        }

        public async Task LogoutAsync()
        {
            try
            {
                var token = await GetAccessTokenAsync();
                if (token != null)
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    await _httpClient.PostAsync($"{BaseUrl}/logout", null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erro ao fazer logout no servidor");
            }
            finally
            {
                SecureStorage.RemoveAll();
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetAccessTokenAsync();
            if (string.IsNullOrEmpty(token))
                return false;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                return jwt.ValidTo > DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            try
            {
                return await SecureStorage.GetAsync(AccessTokenKey);
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> GetUserIdAsync()
        {
            try
            {
                var userIdStr = await SecureStorage.GetAsync(UserIdKey);
                return int.TryParse(userIdStr, out var id) ? id : 0;
            }
            catch
            {
                return 0;
            }
        }

        private async Task StoreTokensAsync(AuthResult result)
        {
            await SecureStorage.SetAsync(AccessTokenKey, result.AccessToken);
            await SecureStorage.SetAsync(RefreshTokenKey, result.RefreshToken);
            await SecureStorage.SetAsync(UserIdKey, result.User.Id);
        }

        private static string? TryExtractError(string responseBody)
        {
            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;

                if (root.TryGetProperty("errors", out var errors))
                {
                    var messages = new List<string>();

                    if (errors.ValueKind == JsonValueKind.Array)
                    {
                        // HttpResult format: errors is string[]
                        foreach (var msg in errors.EnumerateArray())
                            messages.Add(msg.GetString() ?? "Erro desconhecido");
                    }
                    else if (errors.ValueKind == JsonValueKind.Object)
                    {
                        // Validation errors format: errors is { "Field": ["msg1", "msg2"] }
                        foreach (var prop in errors.EnumerateObject())
                        {
                            foreach (var msg in prop.Value.EnumerateArray())
                                messages.Add(msg.GetString() ?? prop.Name);
                        }
                    }

                    if (messages.Count > 0)
                        return string.Join("\n", messages);
                }

                if (root.TryGetProperty("description", out var description))
                    return description.GetString();

                if (root.TryGetProperty("title", out var title))
                    return title.GetString();
            }
            catch { }

            return null;
        }
    }
}
