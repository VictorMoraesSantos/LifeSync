using LifeSyncApp.DTOs.TaskManager.TaskLabel;
using LifeSyncApp.Models.TaskManager;
using LifeSyncApp.Services.ApiService.Implementation;

namespace LifeSyncApp.Services.TaskManager.Implementation
{
    public class TaskLabelService
    {
        private readonly ApiService<TaskLabel> _apiService;
        private const string BaseUrl = "/task-manager/api/task-labels";

        public TaskLabelService(ApiService<TaskLabel> apiService)
        {
            _apiService = apiService;
        }

        public async Task<TaskLabel> GetTaskLabelAsync(int id)
        {
            var result = await _apiService.GetAsync($"{BaseUrl}/{id}");
            return result;
        }

        public async Task<IEnumerable<TaskLabel>> SearchTaskLabelAsync(TaskLabelFilterDTO query)
        {
            var queryParams = new List<string>();

            if (query.Id.HasValue)
                queryParams.Add($"Id={query.Id.Value}");
            if (query.UserId.HasValue)
                queryParams.Add($"UserId={query.UserId.Value}");
            if (query.CreatedAt.HasValue)
                queryParams.Add($"CreatedAt={query.CreatedAt.Value:yyyy-MM-dd}");
            if (query.UpdatedAt.HasValue)
                queryParams.Add($"UpdatedAt={query.UpdatedAt.Value:yyyy-MM-dd}");
            if (query.IsDeleted.HasValue)
                queryParams.Add($"IsDeleted={query.IsDeleted.Value.ToString().ToLower()}");
            if (!string.IsNullOrEmpty(query.SortBy))
                queryParams.Add($"SortBy={Uri.EscapeDataString(query.SortBy)}");
            if (query.SortDesc.HasValue)
                queryParams.Add($"SortDesc={query.SortDesc.Value.ToString().ToLower()}");
            if (query.Page.HasValue)
                queryParams.Add($"Page={query.Page.Value}");
            if (query.PageSize.HasValue)
                queryParams.Add($"PageSize={query.PageSize.Value}");
            if (query.ItemId.HasValue)
                queryParams.Add($"ItemId={query.ItemId.Value}");
            if (query.NameContains is not null)
                queryParams.Add($"NameContains={Uri.EscapeDataString(query.NameContains)}");

            var queryString = string.Join("&", queryParams);
            var result = await _apiService.SearchAsync($"{BaseUrl}?{queryString}");
            return result;
        }

        public async Task<int> CreateTaskLabelAsync(CreateTaskLabelDTO data)
        {
            var result = await _apiService.PostAsync<int>(BaseUrl, data);
            return result;
        }

        public async Task EditTaskLabelAsync(int id, UpdateTaskLabelDTO data)
        {
            await _apiService.PutAsync($"{BaseUrl}/{id}", data);
        }

        public async Task DeleteTaskLabelAsync(int id)
        {
            await _apiService.DeleteAsync($"{BaseUrl}/{id}");
        }
    }
}

