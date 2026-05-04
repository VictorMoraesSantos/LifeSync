using LifeSyncApp.Models.Common;
using LifeSyncApp.Models.Financial.RecurrenceSchedule;
using LifeSyncApp.Models.Financial.Transaction;
using System.Net.Http.Json;
using System.Text.Json;

namespace LifeSyncApp.Services.Financial
{
    public class TransactionService : ITransactionService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string BaseUrl = "/financial-service/api/transactions";

        public TransactionService(IHttpClientFactory httpClientFactory, JsonSerializerOptions jsonOptions)
        {
            _httpClientFactory = httpClientFactory;
            _jsonOptions = jsonOptions;
        }

        public async Task<List<TransactionDTO>> GetTransactionsByUserIdAsync(int userId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{BaseUrl}/user/{userId}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return new List<TransactionDTO>();

                var result =
                    await response.Content.ReadFromJsonAsync<ApiSingleResponse<List<TransactionDTO>>>(_jsonOptions,
                        cancellationToken);
                return result?.Data ?? new List<TransactionDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting transactions: {ex.Message}");
                return new List<TransactionDTO>();
            }
        }

        public async Task<TransactionDTO?> GetTransactionByIdAsync(int id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{BaseUrl}/{id}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return null;

                var result =
                    await response.Content.ReadFromJsonAsync<ApiSingleResponse<TransactionDTO>>(_jsonOptions,
                        cancellationToken);
                return result?.Data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting transaction: {ex.Message}");
                return null;
            }
        }

        public async Task<List<TransactionDTO>> SearchTransactionsAsync(TransactionFilterDTO filter,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var queryString = BuildQueryString(filter);
                var url = $"{BaseUrl}/search?{queryString}";
                System.Diagnostics.Debug.WriteLine($"[TransactionService] GET {client.BaseAddress}{url}");
                var response = await client.GetAsync(url, cancellationToken);

                System.Diagnostics.Debug.WriteLine($"[TransactionService] Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                    System.Diagnostics.Debug.WriteLine($"[TransactionService] Error body: {errorBody}");
                    return new List<TransactionDTO>();
                }

                var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);
                System.Diagnostics.Debug.WriteLine($"[TransactionService] Response JSON (first 500 chars): {rawJson[..Math.Min(500, rawJson.Length)]}");

                var result = JsonSerializer.Deserialize<ApiSingleResponse<List<TransactionDTO>>>(rawJson, _jsonOptions);
                System.Diagnostics.Debug.WriteLine($"[TransactionService] Deserialized {result?.Data?.Count ?? 0} transactions");
                return result?.Data ?? new List<TransactionDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TransactionService] Error searching transactions: {ex.Message}\n{ex.StackTrace}");
                return new List<TransactionDTO>();
            }
        }

        public async Task<(int? Id, string? Error)> CreateTransactionAsync(CreateTransactionDTO dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PostAsJsonAsync(BaseUrl, dto, _jsonOptions, cancellationToken);

                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                System.Diagnostics.Debug.WriteLine(
                    $"[TransactionService] Create response. Status: {response.StatusCode}, Body: {responseBody}");

                if (!response.IsSuccessStatusCode)
                    return (null, ExtractErrorMessage(responseBody));

                // response is 2xx — transaction was created successfully
                int? transactionId = null;
                try
                {
                    using var doc = JsonDocument.Parse(responseBody);
                    var root = doc.RootElement;

                    if (TryGetJsonProperty(root, "data", out var data))
                    {
                        if (data.ValueKind == JsonValueKind.Number && data.TryGetInt32(out var directId))
                            transactionId = directId;
                        else if (data.ValueKind == JsonValueKind.Object &&
                                 TryGetJsonProperty(data, "transactionId", out var txId) &&
                                 txId.TryGetInt32(out var txIdValue))
                            transactionId = txIdValue;
                    }
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine("[TransactionService] Could not parse response body for ID");
                }

                return (transactionId ?? 0, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating transaction: {ex.Message}");
                return (null, ex.Message);
            }
        }

        public async Task<(bool Success, string? Error)> UpdateTransactionAsync(int id, UpdateTransactionDTO dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PutAsJsonAsync($"{BaseUrl}/{id}", dto, _jsonOptions, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    System.Diagnostics.Debug.WriteLine(
                        $"Error updating transaction. Status: {response.StatusCode}, Content: {errorContent}");
                    return (false, ExtractErrorMessage(errorContent));
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating transaction: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Error)> DeleteTransactionAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    System.Diagnostics.Debug.WriteLine($"Error deleting transaction. Status: {response.StatusCode}, Content: {errorContent}");
                    return (false, ExtractErrorMessage(errorContent));
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting transaction: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public async Task<RecurrenceScheduleDTO?> GetScheduleByTransactionIdAsync(int transactionId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var url = $"/financial-service/api/recurrenceschedule/search?TransactionId={transactionId}";
                var response = await client.GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return null;

                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                System.Diagnostics.Debug.WriteLine($"[TransactionService] Schedule response: {responseBody}");

                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;

                if (!TryGetJsonProperty(root, "data", out var data))
                    return null;

                JsonElement firstItem;
                if (data.ValueKind == JsonValueKind.Array)
                {
                    if (data.GetArrayLength() == 0) return null;
                    firstItem = data[0];
                }
                else if (data.ValueKind == JsonValueKind.Object)
                {
                    firstItem = data;
                }
                else
                {
                    return null;
                }

                return JsonSerializer.Deserialize<RecurrenceScheduleDTO>(firstItem.GetRawText(), _jsonOptions);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TransactionService] Error getting schedule: {ex.Message}");
                return null;
            }
        }

        private string BuildQueryString(TransactionFilterDTO filter)
        {
            var parameters = new List<string>();

            if (filter.UserId.HasValue)
                parameters.Add($"UserId={filter.UserId}");
            if (filter.CategoryId.HasValue)
                parameters.Add($"CategoryId={filter.CategoryId}");
            if (filter.PaymentMethod.HasValue)
                parameters.Add($"PaymentMethod={filter.PaymentMethod}");
            if (filter.TransactionType.HasValue)
                parameters.Add($"TransactionType={filter.TransactionType}");
            if (filter.TransactionDateFrom.HasValue)
                parameters.Add($"TransactionDateFrom={filter.TransactionDateFrom:yyyy-MM-dd}");
            if (filter.TransactionDateTo.HasValue)
                parameters.Add($"TransactionDateTo={filter.TransactionDateTo:yyyy-MM-dd}");
            if (filter.Page.HasValue)
                parameters.Add($"Page={filter.Page}");
            if (filter.PageSize.HasValue)
                parameters.Add($"PageSize={filter.PageSize}");

            return string.Join("&", parameters);
        }

        private static bool TryGetJsonProperty(JsonElement element, string propertyName, out JsonElement value)
        {
            if (element.TryGetProperty(propertyName, out value))
                return true;

            // try PascalCase
            var pascal = char.ToUpper(propertyName[0]) + propertyName[1..];
            if (element.TryGetProperty(pascal, out value))
                return true;

            // try case-insensitive
            foreach (var prop in element.EnumerateObject())
            {
                if (string.Equals(prop.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    value = prop.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }

        private static string? ExtractErrorMessage(string responseBody)
        {
            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;

                if (root.TryGetProperty("errors", out var errors))
                {
                    if (errors.ValueKind == JsonValueKind.Array)
                    {
                        var messages = new List<string>();
                        foreach (var item in errors.EnumerateArray())
                            if (item.GetString() is string msg)
                                messages.Add(msg);
                        if (messages.Count > 0)
                            return string.Join("\n", messages);
                    }
                    else if (errors.ValueKind == JsonValueKind.Object)
                    {
                        var messages = new List<string>();
                        foreach (var prop in errors.EnumerateObject())
                            foreach (var msg in prop.Value.EnumerateArray())
                                messages.Add(msg.GetString() ?? prop.Name);
                        if (messages.Count > 0)
                            return string.Join("\n", messages);
                    }
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
