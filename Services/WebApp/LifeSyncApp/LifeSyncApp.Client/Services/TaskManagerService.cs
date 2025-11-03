using LifeSyncApp.Client.Models.Common;
using LifeSyncApp.Client.Models.TaskManager;
using System.Net.Http.Json;

namespace LifeSyncApp.Client.Services
{
    public class TaskManagerService : ITaskManagerService
    {
        private readonly HttpClient _httpClient;

        public TaskManagerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResult<List<TaskItemDto>>> GetTaskItemsAsync(int? userId = null)
        {
            var url = userId.HasValue
                ? $"/taskmanager-service/api/task-items/user/{userId.Value}"
                : "/taskmanager-service/api/task-items";

            var response = await _httpClient.GetAsync(url);
            var result = await response.Content.ReadFromJsonAsync<HttpResult<List<TaskItemDto>>>();
            return result ?? new HttpResult<List<TaskItemDto>> { Success = false };
        }

        public async Task<HttpResult<TaskItemDto>> GetTaskItemByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/taskmanager-service/api/task-items/{id}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult<TaskItemDto>>();
            return result ?? new HttpResult<TaskItemDto> { Success = false };
        }

        public async Task<HttpResult<TaskItemDto>> CreateTaskItemAsync(CreateTaskItemRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/taskmanager-service/api/task-items", request);

            // ? Recebe apenas o ID
            var createResult = await response.Content.ReadFromJsonAsync<HttpResult<int>>();

            if (createResult == null || !createResult.Success)
                return new HttpResult<TaskItemDto> { Success = false };

            // ? Busca a tarefa completa usando o ID retornado
            return await GetTaskItemByIdAsync(createResult.Data);
        }

        public async Task<HttpResult> UpdateTaskItemAsync(int id, UpdateTaskItemRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"/taskmanager-service/api/task-items/{id}", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { Success = false };
        }

        public async Task<HttpResult> DeleteTaskItemAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/taskmanager-service/api/task-items/{id}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { Success = false };
        }

        public async Task<HttpResult<List<TaskLabelDto>>> GetTaskLabelsAsync(int? userId = null)
        {
            var url = userId.HasValue
                ? $"/taskmanager-service/api/task-labels/user/{userId.Value}"
                : "/taskmanager-service/api/task-labels";

            var response = await _httpClient.GetAsync(url);
            var result = await response.Content.ReadFromJsonAsync<HttpResult<List<TaskLabelDto>>>();
            return result ?? new HttpResult<List<TaskLabelDto>> { Success = false };
        }

        public async Task<HttpResult<TaskLabelDto>> GetTaskLabelByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/taskmanager-service/api/task-labels/{id}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult<TaskLabelDto>>();
            return result ?? new HttpResult<TaskLabelDto> { Success = false };
        }

        public async Task<HttpResult<TaskLabelDto>> CreateTaskLabelAsync(CreateTaskLabelRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/taskmanager-service/api/task-labels", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult<TaskLabelDto>>();
            return result ?? new HttpResult<TaskLabelDto> { Success = false };
        }

        public async Task<HttpResult> UpdateTaskLabelAsync(int id, UpdateTaskLabelRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"/taskmanager-service/api/task-labels/{id}", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { Success = false };
        }

        public async Task<HttpResult> DeleteTaskLabelAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/taskmanager-service/api/task-labels/{id}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { Success = false };
        }
    }
}
