using LifeSyncApp.Client.Models.Auth;
using LifeSyncApp.Client.Models.Common;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.Json;

namespace LifeSyncApp.Client.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private const string TokenKey = "authToken";
        private const string UserKey = "currentUser";

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<HttpResult<AuthResult>> LoginAsync(LoginRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/users-service/api/auth/login", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult<AuthResult>>();

            if (result?.Success == true && result.Data != null)
            {
                await SetTokenAsync(result.Data.AccessToken);
                await SetCurrentUserAsync(result.Data.User);
            }

            return result ?? new HttpResult<AuthResult> { Success = false, Errors = new[] { "Erro ao fazer login" } };
        }

        public async Task<HttpResult<AuthResult>> RegisterAsync(RegisterRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/users-service/api/auth/register", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult<AuthResult>>();

            if (result?.Success == true && result.Data != null)
            {
                await SetTokenAsync(result.Data.AccessToken);
                await SetCurrentUserAsync(result.Data.User);
            }

            return result ?? new HttpResult<AuthResult> { Success = false, Errors = new[] { "Erro ao registrar" } };
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync(TokenKey);
            await _localStorage.RemoveItemAsync(UserKey);
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>(TokenKey);
        }

        public async Task SetTokenAsync(string token)
        {
            await _localStorage.SetItemAsync(TokenKey, token);
        }

        public async Task<UserDto?> GetCurrentUserAsync()
        {
            return await _localStorage.GetItemAsync<UserDto>(UserKey);
        }

        public async Task SetCurrentUserAsync(UserDto user)
        {
            await _localStorage.SetItemAsync(UserKey, user);
        }
    }

    public interface ILocalStorageService
    {
        Task<T?> GetItemAsync<T>(string key);
        Task SetItemAsync<T>(string key, T value);
        Task RemoveItemAsync(string key);
    }

    public class LocalStorageService : ILocalStorageService
    {
        private readonly IJSRuntime _jsRuntime;

        public LocalStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<T?> GetItemAsync<T>(string key)
        {
            try
            {
                var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
                if (string.IsNullOrEmpty(json))
                    return default;

                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch
            {
                return default;
            }
        }

        public async Task SetItemAsync<T>(string key, T value)
        {
            try
            {
                var json = JsonSerializer.Serialize(value);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
            }
            catch
            {
                // Ignore errors
            }
        }

        public async Task RemoveItemAsync(string key)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
            }
            catch
            {
                // Ignore errors
            }
        }
    }
}
