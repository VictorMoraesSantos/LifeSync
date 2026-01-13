using LifeSyncApp.DTOs.TaskManager.TaskItem;
using LifeSyncApp.Models.TaskManager;
using LifeSyncApp.Services.ApiService.Implementation;

namespace LifeSyncApp.Services.TaskManager.Implementation
{
    public class TaskItemService
    {
        private readonly ApiService<TaskItem> _apiService;
        private const string BaseUrl = "/taskmanager-service/api/task-items";

        public TaskItemService(ApiService<TaskItem> apiService)
        {
            _apiService = apiService;
        }

        public async Task<TaskItem> GetTaskItemAsync(int id)
        {
            var result = await _apiService.GetAsync($"{BaseUrl}/{id}");
            return result;
        }

        public async Task<IEnumerable<TaskItem>> SearchTaskItemAsync(TaskItemFilterDTO query)
        {
            var queryParams = new List<string>();

            if (query.Id.HasValue)
                queryParams.Add($"Id={query.Id.Value}");
            if (query.UserId.HasValue)
                queryParams.Add($"UserId={query.UserId.Value}");
            if (!string.IsNullOrEmpty(query.TitleContains))
                queryParams.Add($"TitleContains={Uri.EscapeDataString(query.TitleContains)}");
            if (query.Status.HasValue)
                queryParams.Add($"Status={query.Status.Value}");
            if (query.Priority.HasValue)
                queryParams.Add($"Priority={query.Priority.Value}");
            if (query.DueDate.HasValue)
                queryParams.Add($"DueDate={query.DueDate.Value:yyyy-MM-dd}");
            if (query.LabelId.HasValue)
                queryParams.Add($"LabelId={query.LabelId.Value}");
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

            var queryString = string.Join("&", queryParams);
            var result = await _apiService.SearchAsync($"{BaseUrl}/search?{queryString}");
            return result;
        }

        public async Task<TaskItem> CreateTaskItemAsync(CreateTaskItemDTO data)
        {
            var result = await _apiService.PostAsync(BaseUrl, data);
            return result;
        }

        public async Task<TaskItem> EditTaskItemAsync(int id, UpdateTaskItemDTO data)
        {
            var result = await _apiService.PutAsync($"{BaseUrl}/{id}", data);
            return result;
        }

        public async Task DeleteTaskItemAsync(int id)
        {
            await _apiService.DeleteAsync($"{BaseUrl}/{id}");
        }
    }
}

