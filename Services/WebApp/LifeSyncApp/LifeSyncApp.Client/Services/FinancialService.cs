using LifeSyncApp.Client.Models.Common;
using LifeSyncApp.Client.Models.Financial;
using System.Net.Http.Json;
using System.Text.Json;

namespace LifeSyncApp.Client.Services
{
    public class FinancialService : IFinancialService
    {
        private readonly HttpClient _httpClient;

        public FinancialService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Server-side shapes (to match API contracts)
        private sealed class ServerMoney { public int Amount { get; set; } public int Currency { get; set; } }
        private sealed class ServerCategory { public int Id { get; set; } public int UserId { get; set; } public DateTime CreatedAt { get; set; } public DateTime? UpdatedAt { get; set; } public string? Name { get; set; } public string? Description { get; set; } }
        private sealed class ServerTransaction
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public ServerCategory? Category { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public int PaymentMethod { get; set; }
            public int TransactionType { get; set; }
            public ServerMoney Amount { get; set; } = new();
            public string Description { get; set; } = string.Empty;
            public DateTime TransactionDate { get; set; }
            public bool IsRecurring { get; set; }
        }

        public async Task<HttpResult<List<TransactionDto>>> GetTransactionsAsync(int? userId = null)
        {
            var url = userId.HasValue
                ? $"/financial-service/api/transactions/user/{userId.Value}"
                : "/financial-service/api/transactions";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult<List<TransactionDto>>
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            // Read as server shape then map
            var server = await response.Content.ReadFromJsonAsync<HttpResult<List<ServerTransaction>>>();
            if (server == null)
            {
                return new HttpResult<List<TransactionDto>> { Success = false, StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
            }

            var mapped = server.Data?.Select(MapToClient).ToList() ?? new List<TransactionDto>();
            return new HttpResult<List<TransactionDto>>
            {
                Success = server.Success,
                StatusCode = server.StatusCode,
                Errors = server.Errors ?? Array.Empty<string>(),
                Data = mapped
            };
        }

        public async Task<HttpResult<TransactionDto>> GetTransactionByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/financial-service/api/transactions/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult<TransactionDto>
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var server = await response.Content.ReadFromJsonAsync<HttpResult<ServerTransaction>>();
            if (server == null)
            {
                return new HttpResult<TransactionDto> { Success = false, StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
            }

            return new HttpResult<TransactionDto>
            {
                Success = server.Success,
                StatusCode = server.StatusCode,
                Errors = server.Errors ?? Array.Empty<string>(),
                Data = server.Data != null ? MapToClient(server.Data) : null
            };
        }

        public async Task<HttpResult<TransactionDto>> CreateTransactionAsync(CreateTransactionRequest request)
        {
            // Shape payload to match API command (Money value object)
            var payload = new
            {
                userId = request.UserId,
                categoryId = request.CategoryId,
                paymentMethod = request.PaymentMethod,
                transactionType = request.TransactionType,
                amount = new
                {
                    amount = ToWholeAmount(request.Amount),
                    currency = MapCurrencyCode(request.Currency)
                },
                description = request.Description,
                transactionDate = request.TransactionDate,
                isRecurring = request.IsRecurring
            };

            var response = await _httpClient.PostAsJsonAsync("/financial-service/api/transactions", payload);

            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult<TransactionDto>
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            // API returns Created with the new transaction id in Data
            var created = await response.Content.ReadFromJsonAsync<HttpResult<int>>();
            if (created?.Success == true && created.Data > 0)
            {
                var dto = new TransactionDto
                {
                    Id = created.Data,
                    UserId = request.UserId,
                    Category = request.CategoryId.HasValue ? new CategoryDto { Id = request.CategoryId.Value, UserId = request.UserId } : null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    PaymentMethod = request.PaymentMethod,
                    TransactionType = request.TransactionType,
                    Amount = request.Amount,
                    Currency = request.Currency,
                    Description = request.Description,
                    TransactionDate = request.TransactionDate,
                    IsRecurring = request.IsRecurring
                };

                return new HttpResult<TransactionDto>
                {
                    Success = true,
                    StatusCode = 201,
                    Data = dto
                };
            }

            return new HttpResult<TransactionDto>
            {
                Success = false,
                StatusCode = (int)response.StatusCode,
                Errors = created?.Errors ?? new[] { "Não foi possível criar a transação." }
            };
        }

        public async Task<HttpResult> UpdateTransactionAsync(int id, UpdateTransactionRequest request)
        {
            var payload = new
            {
                categoryId = request.CategoryId,
                paymentMethod = request.PaymentMethod,
                transactionType = request.TransactionType,
                amount = new
                {
                    amount = ToWholeAmount(request.Amount),
                    currency = MapCurrencyCode("BRL")
                },
                description = request.Description,
                transactionDate = request.TransactionDate
            };

            var response = await _httpClient.PutAsJsonAsync($"/financial-service/api/transactions/{id}", payload);
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { Success = false };
        }

        public async Task<HttpResult> DeleteTransactionAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/financial-service/api/transactions/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { Success = false };
        }

        public async Task<HttpResult<List<CategoryDto>>> GetCategoriesAsync(int? userId = null)
        {
            var url = userId.HasValue
                ? $"/financial-service/api/categories/user/{userId.Value}"
                : "/financial-service/api/categories";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult<List<CategoryDto>>
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var result = await response.Content.ReadFromJsonAsync<HttpResult<List<CategoryDto>>>();
            return result ?? new HttpResult<List<CategoryDto>> { Success = false };
        }

        public async Task<HttpResult<CategoryDto>> GetCategoryByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/financial-service/api/categories/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult<CategoryDto>
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var result = await response.Content.ReadFromJsonAsync<HttpResult<CategoryDto>>();
            return result ?? new HttpResult<CategoryDto> { Success = false };
        }

        public async Task<HttpResult<CategoryDto>> CreateCategoryAsync(CreateCategoryRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/financial-service/api/categories", request);
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult<CategoryDto>
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var result = await response.Content.ReadFromJsonAsync<HttpResult<CategoryDto>>();
            return result ?? new HttpResult<CategoryDto> { Success = false };
        }

        public async Task<HttpResult> UpdateCategoryAsync(int id, UpdateCategoryRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"/financial-service/api/categories/{id}", request);
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { Success = false };
        }

        public async Task<HttpResult> DeleteCategoryAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/financial-service/api/categories/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { Success = false };
        }

        private static TransactionDto MapToClient(ServerTransaction s) => new()
        {
            Id = s.Id,
            UserId = s.UserId,
            Category = s.Category != null ? new CategoryDto
            {
                Id = s.Category.Id,
                UserId = s.Category.UserId,
                CreatedAt = s.Category.CreatedAt,
                UpdatedAt = s.Category.UpdatedAt,
                Name = s.Category.Name ?? string.Empty,
                Description = s.Category.Description
            } : null,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt,
            PaymentMethod = s.PaymentMethod,
            TransactionType = s.TransactionType,
            Amount = FromWholeAmount(s.Amount.Amount),
            Currency = FromCurrencyCode(s.Amount.Currency),
            Description = s.Description,
            TransactionDate = s.TransactionDate,
            IsRecurring = s.IsRecurring
        };

        private static decimal FromWholeAmount(int amount)
        {
            // Se seu domínio usa centavos, troque para amount/100m
            return amount;
        }

        private static string FromCurrencyCode(int code) => code switch
        {
            0 => "USD",
            1 => "EUR",
            2 => "BRL",
            _ => "BRL"
        };

        private static int ToWholeAmount(decimal amount)
        {
            // Ajuste conforme seu domínio (centavos x unidade). Aqui arredonda para inteiro.
            return (int)Math.Round(amount, MidpointRounding.AwayFromZero);
        }

        private static int MapCurrencyCode(string? code)
        {
            // Mapeamento mínimo; ajuste se sua UI suportar mais moedas.
            return (code ?? "").ToUpperInvariant() switch
            {
                "USD" => 0,
                "EUR" => 1,
                "BRL" => 2,
                _ => 2
            };
        }

        private static async Task<string[]> ReadErrorsAsync(HttpResponseMessage response)
        {
            try
            {
                var json = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json)) return new[] { response.ReasonPhrase ?? "Erro" };

                using var doc = JsonDocument.Parse(json);
                // Tenta HttpResult shape => { errors: ["..."] }
                if (doc.RootElement.TryGetProperty("errors", out var errorsProp))
                {
                    if (errorsProp.ValueKind == JsonValueKind.Array)
                    {
                        return errorsProp.EnumerateArray().Select(e => e.GetString() ?? string.Empty).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                    }
                    // ProblemDetails shape => errors: { field: ["msg"] }
                    if (errorsProp.ValueKind == JsonValueKind.Object)
                    {
                        var list = new List<string>();
                        foreach (var prop in errorsProp.EnumerateObject())
                        {
                            if (prop.Value.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var item in prop.Value.EnumerateArray())
                                {
                                    var msg = item.GetString();
                                    if (!string.IsNullOrWhiteSpace(msg)) list.Add(msg!);
                                }
                            }
                        }
                        return list.Count > 0 ? list.ToArray() : new[] { "Requisição inválida" };
                    }
                }
                // fallback: try title/detail
                var title = doc.RootElement.TryGetProperty("title", out var t) ? t.GetString() : null;
                var detail = doc.RootElement.TryGetProperty("detail", out var d) ? d.GetString() : null;
                return new[] { detail ?? title ?? response.ReasonPhrase ?? "Erro" };
            }
            catch
            {
                return new[] { response.ReasonPhrase ?? "Erro" };
            }
        }
    }
}
