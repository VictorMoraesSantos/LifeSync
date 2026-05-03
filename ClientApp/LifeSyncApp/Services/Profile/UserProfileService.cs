using LifeSyncApp.DTOs.Auth;
using LifeSyncApp.DTOs.Profile;
using System.Net.Http.Json;
using System.Text.Json;

namespace LifeSyncApp.Services.Profile
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string UsersBaseUrl = "/users-service/api/users";
        private const string AuthBaseUrl = "/auth";

        public UserProfileService(IHttpClientFactory httpClientFactory, JsonSerializerOptions jsonOptions)
        {
            _httpClientFactory = httpClientFactory;
            _jsonOptions = jsonOptions;
        }

        public async Task<UserDTO?> GetUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{UsersBaseUrl}/{userId}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return null;

                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                System.Diagnostics.Debug.WriteLine($"[UserProfileService] GET user response: {body}");

                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                if (!root.TryGetProperty("data", out var data))
                    return null;

                // Backend wraps in GetUserQueryResult: { "data": { "user": { ... } } }
                var userElement = data.TryGetProperty("user", out var user) ? user : data;

                return JsonSerializer.Deserialize<UserDTO>(userElement.GetRawText(), _jsonOptions);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UserProfileService] Error getting user: {ex.Message}");
                return null;
            }
        }

        public async Task<(bool Success, string? Error)> UpdateUserAsync(int userId, UpdateUserRequest dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                System.Diagnostics.Debug.WriteLine($"[UserProfileService] PUT {UsersBaseUrl}/{userId}");

                var response = await client.PutAsJsonAsync($"{UsersBaseUrl}/{userId}", dto, _jsonOptions, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    System.Diagnostics.Debug.WriteLine($"[UserProfileService] Error: {response.StatusCode}, Content: {errorContent}");
                    return (false, ExtractErrorMessage(errorContent));
                }

                System.Diagnostics.Debug.WriteLine($"[UserProfileService] User {userId} updated successfully");
                return (true, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UserProfileService] Error updating user: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Error)> ChangePasswordAsync(ChangePasswordRequest dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                System.Diagnostics.Debug.WriteLine($"[UserProfileService] POST {AuthBaseUrl}/change-password");

                var response = await client.PostAsJsonAsync($"{AuthBaseUrl}/change-password", dto, _jsonOptions, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    System.Diagnostics.Debug.WriteLine($"[UserProfileService] Error: {response.StatusCode}, Content: {errorContent}");
                    return (false, ExtractErrorMessage(errorContent));
                }

                System.Diagnostics.Debug.WriteLine("[UserProfileService] Password changed successfully");
                return (true, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UserProfileService] Error changing password: {ex.Message}");
                return (false, ex.Message);
            }
        }

        private static string? ExtractErrorMessage(string responseBody)
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
                        foreach (var msg in errors.EnumerateArray())
                            messages.Add(msg.GetString() ?? "Erro desconhecido");
                    }
                    else if (errors.ValueKind == JsonValueKind.Object)
                    {
                        foreach (var prop in errors.EnumerateObject())
                            foreach (var msg in prop.Value.EnumerateArray())
                                messages.Add(msg.GetString() ?? prop.Name);
                    }

                    if (messages.Count > 0)
                        return string.Join("\n", messages);
                }

                if (root.TryGetProperty("description", out var desc) && desc.GetString() is string d)
                    return d;
                if (root.TryGetProperty("title", out var title) && title.GetString() is string t)
                    return t;
            }
            catch { }
            return null;
        }
    }
}
