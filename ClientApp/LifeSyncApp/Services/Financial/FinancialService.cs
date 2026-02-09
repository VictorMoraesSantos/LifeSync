using System.Net.Http.Json;
using System.Text.Json;
using LifeSyncApp.DTOs.Financial;
using LifeSyncApp.Models.Financial;

namespace LifeSyncApp.Services.Financial;

public class FinancialService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private const string BaseEndpoint = "/api/financial/transactions";

    public FinancialService(IHttpClientFactory httpClientFactory, JsonSerializerOptions jsonOptions)
    {
        _httpClientFactory = httpClientFactory;
        _jsonOptions = jsonOptions;
    }

    private HttpClient CreateClient()
    {
        return _httpClientFactory.CreateClient("LifeSyncApi");
    }

    public async Task<TransactionDTO?> GetTransactionByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync($"{BaseEndpoint}/{id}", cancellationToken);
            
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine($"Response JSON: {json}");

            var result = await response.Content.ReadFromJsonAsync<HttpResultWrapper>(_jsonOptions, cancellationToken);
            
            if (result?.Data == null)
                return null;

            return JsonSerializer.Deserialize<TransactionDTO>(result.Data.ToString()!, _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar transa\u00e7\u00e3o: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            return null;
        }
    }

    public async Task<List<TransactionDTO>> GetTransactionsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync($"{BaseEndpoint}/user/{userId}", cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Status Code: {response.StatusCode}");
                return new List<TransactionDTO>();
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine($"Response JSON: {json}");

            var result = await response.Content.ReadFromJsonAsync<HttpResultWrapper>(_jsonOptions, cancellationToken);
            
            if (result?.Data == null)
                return new List<TransactionDTO>();

            var dataJson = result.Data.ToString();
            return JsonSerializer.Deserialize<List<TransactionDTO>>(dataJson!, _jsonOptions) ?? new List<TransactionDTO>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar transa\u00e7\u00f5es do usu\u00e1rio: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            return new List<TransactionDTO>();
        }
    }

    public async Task<PaginatedResult<TransactionDTO>> SearchTransactionsAsync(TransactionFilterDTO filter, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = CreateClient();
            var queryParams = BuildQueryString(filter);
            var response = await client.GetAsync($"{BaseEndpoint}/search?{queryParams}", cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Status Code: {response.StatusCode}");
                return new PaginatedResult<TransactionDTO>();
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine($"Search Response: {json}");

            var result = await response.Content.ReadFromJsonAsync<HttpResultWrapperPaginated>(_jsonOptions, cancellationToken);
            
            if (result?.Data == null)
                return new PaginatedResult<TransactionDTO>();

            var dataJson = result.Data.ToString();
            var items = JsonSerializer.Deserialize<List<TransactionDTO>>(dataJson!, _jsonOptions) ?? new List<TransactionDTO>();
            
            return new PaginatedResult<TransactionDTO>
            {
                Items = items,
                TotalCount = result.Pagination?.TotalCount ?? items.Count,
                Page = result.Pagination?.Page ?? 1,
                PageSize = result.Pagination?.PageSize ?? items.Count
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar transa\u00e7\u00f5es: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            return new PaginatedResult<TransactionDTO>();
        }
    }

    public async Task<List<TransactionDTO>> GetAllTransactionsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync(BaseEndpoint, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
                return new List<TransactionDTO>();

            var result = await response.Content.ReadFromJsonAsync<HttpResultWrapper>(_jsonOptions, cancellationToken);
            
            if (result?.Data == null)
                return new List<TransactionDTO>();

            var dataJson = result.Data.ToString();
            return JsonSerializer.Deserialize<List<TransactionDTO>>(dataJson!, _jsonOptions) ?? new List<TransactionDTO>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar todas transa\u00e7\u00f5es: {ex.Message}");
            return new List<TransactionDTO>();
        }
    }

    public async Task<int?> CreateTransactionAsync(CreateTransactionDTO dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = CreateClient();
            
            // Criar command para a API
            var command = new
            {
                userId = dto.UserId,
                categoryId = dto.CategoryId,
                paymentMethod = (int)dto.PaymentMethod,
                transactionType = (int)dto.TransactionType,
                amount = new { amount = dto.Amount, currency = dto.Currency },
                description = dto.Description,
                transactionDate = dto.TransactionDate.ToString("yyyy-MM-ddTHH:mm:ss")
            };

            var response = await client.PostAsJsonAsync(BaseEndpoint, command, _jsonOptions, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine($"Erro ao criar: {error}");
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<HttpResultWrapper>(_jsonOptions, cancellationToken);
            
            if (result?.Data == null)
                return null;

            // Tentar extrair o ID da resposta
            var dataJson = result.Data.ToString();
            if (int.TryParse(dataJson, out var id))
                return id;

            // Tentar deserializar como objeto
            var responseObj = JsonSerializer.Deserialize<CreateTransactionResponse>(dataJson!, _jsonOptions);
            return responseObj?.TransactionId;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar transa\u00e7\u00e3o: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            return null;
        }
    }

    public async Task<bool> UpdateTransactionAsync(int id, UpdateTransactionDTO dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = CreateClient();
            
            var command = new
            {
                categoryId = dto.CategoryId,
                paymentMethod = (int)dto.PaymentMethod,
                transactionType = (int)dto.TransactionType,
                amount = new { amount = dto.Amount, currency = dto.Currency },
                description = dto.Description,
                transactionDate = dto.TransactionDate.ToString("yyyy-MM-ddTHH:mm:ss")
            };

            var response = await client.PutAsJsonAsync($"{BaseEndpoint}/{id}", command, _jsonOptions, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar transa\u00e7\u00e3o: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteTransactionAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = CreateClient();
            var response = await client.DeleteAsync($"{BaseEndpoint}/{id}", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao excluir transa\u00e7\u00e3o: {ex.Message}");
            return false;
        }
    }

    private string BuildQueryString(TransactionFilterDTO filter)
    {
        var parameters = new List<string>();

        if (filter.UserId.HasValue)
            parameters.Add($"userId={filter.UserId}");
        
        if (filter.CategoryId.HasValue)
            parameters.Add($"categoryId={filter.CategoryId}");
        
        if (filter.TransactionType.HasValue)
            parameters.Add($"transactionType={filter.TransactionType}");
        
        if (filter.PaymentMethod.HasValue)
            parameters.Add($"paymentMethod={filter.PaymentMethod}");
        
        if (filter.StartDate.HasValue)
            parameters.Add($"startDate={filter.StartDate:yyyy-MM-dd}");
        
        if (filter.EndDate.HasValue)
            parameters.Add($"endDate={filter.EndDate:yyyy-MM-dd}");
        
        if (filter.MinAmount.HasValue)
            parameters.Add($"minAmount={filter.MinAmount}");
        
        if (filter.MaxAmount.HasValue)
            parameters.Add($"maxAmount={filter.MaxAmount}");
        
        parameters.Add($"page={filter.Page}");
        parameters.Add($"pageSize={filter.PageSize}");
        
        if (!string.IsNullOrEmpty(filter.SortBy))
            parameters.Add($"sortBy={filter.SortBy}");
        
        parameters.Add($"sortDescending={filter.SortDescending}");

        return string.Join("&", parameters);
    }
}

// Response wrappers para API
public class HttpResultWrapper
{
    public object? Data { get; set; }
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public int StatusCode { get; set; }
}

public class HttpResultWrapperPaginated
{
    public object? Data { get; set; }
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public int StatusCode { get; set; }
    public PaginationInfo? Pagination { get; set; }
}

public class PaginationInfo
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}

public class CreateTransactionResponse
{
    public int TransactionId { get; set; }
}
