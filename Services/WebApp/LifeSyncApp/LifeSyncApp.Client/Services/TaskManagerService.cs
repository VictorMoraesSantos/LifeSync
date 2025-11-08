using LifeSyncApp.Client.Models;
using LifeSyncApp.Client.Models.TaskManager;
using LifeSyncApp.Client.Models.TaskManager.TaskItem;
using LifeSyncApp.Client.Models.TaskManager.TaskLabel;
using LifeSyncApp.Client.Services.Http;

namespace LifeSyncApp.Client.Services
{
    // Task Manager Service aligned with backend DTOs
    public interface ITaskManagerService
    {
        // TaskItems
        Task<ApiResponse<List<TaskItemDTO>>> GetTasksAsync();
        Task<ApiResponse<TaskItemDTO>> GetTaskByIdAsync(int id);
        Task<ApiResponse<int>> CreateTaskAsync(CreateTaskItemCommand command);
        Task<ApiResponse<bool>> UpdateTaskAsync(UpdateTaskItemCommand command);
        Task<ApiResponse<object>> DeleteTaskAsync(int id);

        // Labels
        Task<ApiResponse<List<TaskLabelDTO>>> GetLabelsAsync();
        Task<ApiResponse<int>> CreateLabelAsync(CreateTaskLabelCommand command);
        Task<ApiResponse<bool>> UpdateLabelAsync(UpdateTaskLabelCommand command);
        Task<ApiResponse<object>> DeleteLabelAsync(int id);

        // Search and by-user
        Task<ApiResponse<List<TaskItemDTO>>> SearchTasksAsync(TaskItemFilterDTO filter);
        Task<ApiResponse<List<TaskLabelDTO>>> SearchLabelsAsync(TaskLabelFilterDTO filter);
        Task<ApiResponse<List<TaskItemDTO>>> GetTasksByUserAsync(int userId);
        Task<ApiResponse<List<TaskLabelDTO>>> GetLabelsByUserAsync(int userId);
    }

    public class TaskManagerService : ITaskManagerService
    {
        private readonly IApiClient _apiClient;

        public TaskManagerService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<ApiResponse<List<TaskItemDTO>>> GetTasksAsync()
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<TaskItemDTO>>>("taskmanager-service/api/task-items");
                return res ?? new ApiResponse<List<TaskItemDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TaskItemDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<TaskItemDTO>> GetTaskByIdAsync(int id)
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<TaskItemDTO>>($"taskmanager-service/api/task-items/{id}");
                return res ?? new ApiResponse<TaskItemDTO> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<TaskItemDTO> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<int>> CreateTaskAsync(CreateTaskItemCommand command)
        {
            try
            {
                var res = await _apiClient.PostAsync<CreateTaskItemCommand, ApiResponse<int>>("taskmanager-service/api/task-items", command);
                return res ?? new ApiResponse<int> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<bool>> UpdateTaskAsync(UpdateTaskItemCommand command)
        {
            try
            {
                var res = await _apiClient.PutAsync<UpdateTaskItemCommand, ApiResponse<bool>>($"taskmanager-service/api/task-items/{command.Id}", command);
                return res ?? new ApiResponse<bool> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<object>> DeleteTaskAsync(int id)
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

        public async Task<ApiResponse<List<TaskLabelDTO>>> GetLabelsAsync()
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<TaskLabelDTO>>>("taskmanager-service/api/task-labels");
                return res ?? new ApiResponse<List<TaskLabelDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TaskLabelDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<int>> CreateLabelAsync(CreateTaskLabelCommand command)
        {
            try
            {
                var res = await _apiClient.PostAsync<CreateTaskLabelCommand, ApiResponse<int>>("taskmanager-service/api/task-labels", command);
                return res ?? new ApiResponse<int> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<bool>> UpdateLabelAsync(UpdateTaskLabelCommand command)
        {
            try
            {
                var res = await _apiClient.PutAsync<UpdateTaskLabelCommand, ApiResponse<bool>>($"taskmanager-service/api/task-labels/{command.Id}", command);
                return res ?? new ApiResponse<bool> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<object>> DeleteLabelAsync(int id)
        {
            try
            {
                await _apiClient.DeleteAsync($"taskmanager-service/api/task-labels/{id}");
                return new ApiResponse<object> { Success = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<TaskItemDTO>>> SearchTasksAsync(TaskItemFilterDTO filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var res = await _apiClient.GetAsync<ApiResponse<List<TaskItemDTO>>>($"taskmanager-service/api/task-items/search{query}");
                return res ?? new ApiResponse<List<TaskItemDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TaskItemDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<TaskLabelDTO>>> SearchLabelsAsync(TaskLabelFilterDTO filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var res = await _apiClient.GetAsync<ApiResponse<List<TaskLabelDTO>>>($"taskmanager-service/api/task-labels/search{query}");
                return res ?? new ApiResponse<List<TaskLabelDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TaskLabelDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<TaskItemDTO>>> GetTasksByUserAsync(int userId)
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<TaskItemDTO>>>($"taskmanager-service/api/task-items/user/{userId}");
                return res ?? new ApiResponse<List<TaskItemDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TaskItemDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<TaskLabelDTO>>> GetLabelsByUserAsync(int userId)
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<TaskLabelDTO>>>($"taskmanager-service/api/task-labels/user/{userId}");
                return res ?? new ApiResponse<List<TaskLabelDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TaskLabelDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }
    }
}
