using System.Net.Http.Json;
using System.Text.Json;
using LifeSyncApp.DTOs.Financial;
using LifeSyncApp.Models.Financial;

namespace LifeSyncApp.Services.Financial;

public class FinancialService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private const string BaseEndpoint = "/api/transactions";

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

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<TransactionDTO>>(_jsonOptions, cancellationToken);
            return result?.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar transação: {ex.Message}");
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
                return new List<TransactionDTO>();

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<TransactionDTO>>>(_jsonOptions, cancellationToken);
            return result?.Data ?? new List<TransactionDTO>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar transações do usuário: {ex.Message}");
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
                return new PaginatedResult<TransactionDTO>();

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResult<TransactionDTO>>>(_jsonOptions, cancellationToken);
            return result?.Data ?? new PaginatedResult<TransactionDTO>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar transações: {ex.Message}");
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

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<TransactionDTO>>>(_jsonOptions, cancellationToken);
            return result?.Data ?? new List<TransactionDTO>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar todas transações: {ex.Message}");
            return new List<TransactionDTO>();
        }
    }

    public async Task<int?> CreateTransactionAsync(CreateTransactionDTO dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync(BaseEndpoint, dto, _jsonOptions, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<CreateTransactionResponse>>(_jsonOptions, cancellationToken);
            return result?.Data?.TransactionId;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar transação: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateTransactionAsync(int id, UpdateTransactionDTO dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = CreateClient();
            var response = await client.PutAsJsonAsync($"{BaseEndpoint}/{id}", dto, _jsonOptions, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar transação: {ex.Message}");
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
            Console.WriteLine($"Erro ao excluir transação: {ex.Message}");
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

// Response wrappers
public class ApiResponse<T>
{
    public T? Data { get; set; }
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
}

public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}

public class CreateTransactionResponse
{
    public int TransactionId { get; set; }
}
