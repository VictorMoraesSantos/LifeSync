using LifeSyncApp.Client.Models;
using LifeSyncApp.Client.Models.TaskManager.TaskItem;
using LifeSyncApp.Client.Services.Contracts;
using LifeSyncApp.Client.Services.Http;
using LifeSyncApp.Client.Services.TaskManager.Contrats;

namespace LifeSyncApp.Client.Services.TaskManager
{
    public class TaskItemService : ITaskItemService
    {
        private readonly IApiClient _apiClient;
        private const string BaseRoute = "taskmanager-service/api/task-items";

        public TaskItemService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<ApiResponse<List<TaskItemDTO>>> GetTasksAsync()
        {
            try
            {
                var response = await _apiClient.GetAsync<ApiResponse<List<TaskItemDTO>>>(BaseRoute);
                return response ?? new ApiResponse<List<TaskItemDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TaskItemDTO>>
                {
                    Success = false,
                    Errors = new[] { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<TaskItemDTO>> GetTaskByIdAsync(int id)
        {
            try
            {
                var response = await _apiClient.GetAsync<ApiResponse<TaskItemDTO>>($"{BaseRoute}/{id}");
                return response ?? new ApiResponse<TaskItemDTO> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<TaskItemDTO>
                {
                    Success = false,
                    Errors = new[] { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<int>> CreateTaskAsync(CreateTaskItemDTO command)
        {
            try
            {
                var response = await _apiClient.PostAsync<CreateTaskItemDTO, ApiResponse<int>>(
                    BaseRoute,
                    command);
                return response ?? new ApiResponse<int> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int>
                {
                    Success = false,
                    Errors = new[] { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<bool>> UpdateTaskAsync(UpdateTaskItemDTO command)
        {
            try
            {
                var response = await _apiClient.PutAsync<UpdateTaskItemDTO, ApiResponse<bool>>(
                    $"{BaseRoute}/{command.Id}",
                    command);
                return response ?? new ApiResponse<bool> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Errors = new[] { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<object>> DeleteTaskAsync(int id)
        {
            try
            {
                await _apiClient.DeleteAsync($"{BaseRoute}/{id}");
                return new ApiResponse<object> { Success = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Errors = new[] { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<List<TaskItemDTO>>> SearchTasksAsync(TaskItemFilterDTO filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var response = await _apiClient.GetAsync<ApiResponse<List<TaskItemDTO>>>(
                    $"{BaseRoute}/search{query}");
                return response ?? new ApiResponse<List<TaskItemDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TaskItemDTO>>
                {
                    Success = false,
                    Errors = new[] { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<List<TaskItemDTO>>> GetTasksByUserAsync(int userId)
        {
            try
            {
                var response = await _apiClient.GetAsync<ApiResponse<List<TaskItemDTO>>>(
                    $"{BaseRoute}/user/{userId}");
                return response ?? new ApiResponse<List<TaskItemDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TaskItemDTO>>
                {
                    Success = false,
                    Errors = new[] { ex.Message }
                };
            }
        }
    }
}
