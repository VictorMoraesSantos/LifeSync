using LifeSyncApp.Client.Services;

namespace LifeSyncApp.Services
{
    // No-op local storage for server/prerendering
    public sealed class ServerLocalStorageService : ILocalStorageService
    {
        public Task<T?> GetItemAsync<T>(string key) => Task.FromResult(default(T));
        public Task SetItemAsync<T>(string key, T value) => Task.CompletedTask;
        public Task RemoveItemAsync(string key) => Task.CompletedTask;
    }
}