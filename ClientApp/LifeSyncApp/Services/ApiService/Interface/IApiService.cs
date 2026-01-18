namespace LifeSyncApp.Services.ApiService.Interface
{
    public interface IApiService<T>
    {
        Task<T> GetAsync(string endpoint);
        Task<IEnumerable<T>> SearchAsync(string endpoint);
        Task<TResult> PostAsync<TResult>(string endpoint, object data);
        Task PutAsync(string endpoint, object data);
        Task DeleteAsync(string endpoint);
    }
}
