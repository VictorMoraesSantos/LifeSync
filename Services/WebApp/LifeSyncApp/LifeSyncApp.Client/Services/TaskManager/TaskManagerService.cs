using LifeSyncApp.Client.Models;
using LifeSyncApp.Client.Models.TaskManager.TaskItem;
using LifeSyncApp.Client.Models.TaskManager.TaskLabel;
using LifeSyncApp.Client.Services.Contracts;
using LifeSyncApp.Client.Services.Http;

namespace LifeSyncApp.Client.Services.TaskManager
{
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

        public async Task<ApiResponse<int>> CreateTaskAsync(CreateTaskItemDTO command)
        {
            try
            {
                var res = await _apiClient.PostAsync<CreateTaskItemDTO, ApiResponse<int>>("taskmanager-service/api/task-items", command);
                return res ?? new ApiResponse<int> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<bool>> UpdateTaskAsync(UpdateTaskItemDTO command)
        {
            try
            {
                var res = await _apiClient.PutAsync<UpdateTaskItemDTO, ApiResponse<bool>>($"taskmanager-service/api/task-items/{command.Id}", command);
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

        public async Task<ApiResponse<int>> CreateLabelAsync(CreateTaskLabelDTO command)
        {
            try
            {
                var res = await _apiClient.PostAsync<CreateTaskLabelDTO, ApiResponse<int>>("taskmanager-service/api/task-labels", command);
                return res ?? new ApiResponse<int> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<bool>> UpdateLabelAsync(UpdateTaskLabelDTO command)
        {
            try
            {
                var res = await _apiClient.PutAsync<UpdateTaskLabelDTO, ApiResponse<bool>>($"taskmanager-service/api/task-labels/{command.Id}", command);
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
