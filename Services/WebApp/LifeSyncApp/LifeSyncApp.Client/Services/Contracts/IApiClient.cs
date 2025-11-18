namespace LifeSyncApp.Client.Services.Contracts
{
    public interface IApiClient
    {
        Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default);
        Task<TResult?> PostAsync<TRequest, TResult>(string url, TRequest body, CancellationToken cancellationToken = default);
        Task<TResult?> PutAsync<TRequest, TResult>(string url, TRequest body, CancellationToken cancellationToken = default);
        Task DeleteAsync(string url, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
    }
}
