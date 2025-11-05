using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using LifeSyncApp.Client.Authentication;

namespace LifeSyncApp.Client.Services.Http;

public interface IApiClient
{
    Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default);
    Task<TResult?> PostAsync<TRequest, TResult>(string url, TRequest body, CancellationToken cancellationToken = default);
    Task<TResult?> PutAsync<TRequest, TResult>(string url, TRequest body, CancellationToken cancellationToken = default);
    Task DeleteAsync(string url, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
}

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly CustomAuthStateProvider _authProvider;

    public ApiClient(HttpClient httpClient, CustomAuthStateProvider authProvider)
    {
        _httpClient = httpClient;
        _authProvider = authProvider;
    }

    public async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
    }

    public async Task<TResult?> PostAsync<TRequest, TResult>(string url, TRequest body, CancellationToken cancellationToken = default)
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.PostAsJsonAsync(url, body, cancellationToken);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<TResult>(cancellationToken: cancellationToken);
    }

    public async Task<TResult?> PutAsync<TRequest, TResult>(string url, TRequest body, CancellationToken cancellationToken = default)
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.PutAsJsonAsync(url, body, cancellationToken);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<TResult>(cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.DeleteAsync(url, cancellationToken);
        await EnsureSuccessAsync(response);
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        await AddAuthHeaderAsync();
        return await _httpClient.SendAsync(request, cancellationToken);
    }

    private async Task AddAuthHeaderAsync()
    {
        var token = await _authProvider.GetTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        var content = response.Content == null ? null : await response.Content.ReadAsStringAsync();
        throw new ApiException(response.StatusCode, content);       
    }
}

public class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string? ResponseContent { get; }

    public ApiException(HttpStatusCode statusCode, string? responseContent, string? message = null)
        : base(message ?? $"API request failed with status {(int)statusCode} ({statusCode}).")
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
    }
}
