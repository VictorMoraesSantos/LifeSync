using LifeSyncApp.DTOs.Financial;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace LifeSyncApp.Services.Financial;

public class FinancialService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public FinancialService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:5001/api/"); // Financial API
        _httpClient.Timeout = TimeSpan.FromSeconds(30);

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
    }

    public async Task<List<TransactionDTO>> GetAllTransactionsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<TransactionDTO>>("transactions", _jsonOptions);
            return response ?? new List<TransactionDTO>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar transações: {ex.Message}");
            return new List<TransactionDTO>();
        }
    }

    public async Task<TransactionDTO?> GetTransactionByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<TransactionDTO>($"transactions/{id}", _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar transação: {ex.Message}");
            return null;
        }
    }

    public async Task<int?> CreateTransactionAsync(CreateTransactionDTO dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("transactions", dto, _jsonOptions);
            response.EnsureSuccessStatusCode();
            
            var created = await response.Content.ReadFromJsonAsync<TransactionDTO>(_jsonOptions);
            return created?.Id;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar transação: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateTransactionAsync(int id, UpdateTransactionDTO dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"transactions/{id}", dto, _jsonOptions);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar transação: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteTransactionAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"transactions/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao excluir transação: {ex.Message}");
            return false;
        }
    }
}
