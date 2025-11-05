using LifeSyncApp.Client.Models;
using LifeSyncApp.Client.Services.Http;
using System.Net.Http.Json;

namespace LifeSyncApp.Client.Services
{
    // Task Manager Models
    public class TaskItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; } = "Medium";
        public string Status { get; set; } = "Pending";
        public List<string> Labels { get; set; } = new();
    }

    public class TaskLabel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = "#000000";
    }

    // Lightweight filter models that match API query parameter names
    public class TaskItemFilter
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public string? TitleContains { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public DateOnly? DueDate { get; set; }
        public int? LabelId { get; set; }
        public DateOnly? CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
        public bool? IsDeleted { get; set; }
        public string? SortBy { get; set; }
        public bool? SortDesc { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

    public class TaskLabelFilter
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public int? TaskItemId { get; set; }
        public string? NameContains { get; set; }
        public string? LabelColor { get; set; }
        public DateOnly? CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
        public bool? IsDeleted { get; set; }
        public string? SortBy { get; set; }
        public bool? SortDesc { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

    // Task Manager Service
    public interface ITaskManagerService
    {
        Task<ApiResponse<List<TaskItem>>> GetTasksAsync();
        Task<ApiResponse<TaskItem>> GetTaskByIdAsync(Guid id);
        Task<ApiResponse<TaskItem>> CreateTaskAsync(TaskItem task);
        Task<ApiResponse<TaskItem>> UpdateTaskAsync(Guid id, TaskItem task);
        Task<ApiResponse<object>> DeleteTaskAsync(Guid id);
        Task<ApiResponse<List<TaskLabel>>> GetLabelsAsync();

        // New search endpoints
        Task<ApiResponse<List<TaskItem>>> SearchTasksAsync(TaskItemFilter filter);
        Task<ApiResponse<List<TaskLabel>>> SearchLabelsAsync(TaskLabelFilter filter);
        Task<ApiResponse<List<TaskItem>>> GetTasksByUserAsync(int userId);
        Task<ApiResponse<List<TaskLabel>>> GetLabelsByUserAsync(int userId);
    }

    public class TaskManagerService : ITaskManagerService
    {
        private readonly IApiClient _apiClient;

        public TaskManagerService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<ApiResponse<List<TaskItem>>> GetTasksAsync()
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<TaskItem>>>("taskmanager-service/api/task-items");
                return res ?? new ApiResponse<List<TaskItem>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TaskItem>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<TaskItem>> GetTaskByIdAsync(Guid id)
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<TaskItem>>($"taskmanager-service/api/task-items/{id}");
                return res ?? new ApiResponse<TaskItem> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<TaskItem> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<TaskItem>> CreateTaskAsync(TaskItem task)
        {
            try
            {
                var res = await _apiClient.PostAsync<TaskItem, ApiResponse<TaskItem>>("taskmanager-service/api/task-items", task);
                return res ?? new ApiResponse<TaskItem> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<TaskItem> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<TaskItem>> UpdateTaskAsync(Guid id, TaskItem task)
        {
            try
            {
                var res = await _apiClient.PutAsync<TaskItem, ApiResponse<TaskItem>>($"taskmanager-service/api/task-items/{id}", task);
                return res ?? new ApiResponse<TaskItem> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<TaskItem> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<object>> DeleteTaskAsync(Guid id)
        {
            try
            {
                await _apiClient.DeleteAsync($"taskmanager-service/api/task-items/{id}");
                return new ApiResponse<object> { Success = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<TaskLabel>>> GetLabelsAsync()
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<TaskLabel>>>("taskmanager-service/api/task-labels");
                return res ?? new ApiResponse<List<TaskLabel>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TaskLabel>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<TaskItem>>> SearchTasksAsync(TaskItemFilter filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var res = await _apiClient.GetAsync<ApiResponse<List<TaskItem>>>($"taskmanager-service/api/task-items/search{query}");
                return res ?? new ApiResponse<List<TaskItem>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TaskItem>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<TaskLabel>>> SearchLabelsAsync(TaskLabelFilter filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var res = await _apiClient.GetAsync<ApiResponse<List<TaskLabel>>>($"taskmanager-service/api/task-labels/search{query}");
                return res ?? new ApiResponse<List<TaskLabel>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TaskLabel>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<TaskItem>>> GetTasksByUserAsync(int userId)
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<TaskItem>>>($"taskmanager-service/api/task-items/user/{userId}");
                return res ?? new ApiResponse<List<TaskItem>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TaskItem>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<TaskLabel>>> GetLabelsByUserAsync(int userId)
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<TaskLabel>>>($"taskmanager-service/api/task-labels/user/{userId}");
                return res ?? new ApiResponse<List<TaskLabel>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TaskLabel>> { Success = false, Errors = new[] { ex.Message } };
            }
        }
    }
}
