using LifeSyncApp.Client.Models;
using LifeSyncApp.Client.Models.TaskManager.TaskLabel;
using LifeSyncApp.Client.Services.Contracts;
using LifeSyncApp.Client.Services.Http;
using LifeSyncApp.Client.Services.TaskManager.Contrats;

namespace LifeSyncApp.Client.Services.TaskManager
{
    public class TaskLabelService : ITaskLabelService
    {
        private readonly IApiClient _apiClient;
        private const string BaseRoute = "taskmanager-service/api/task-labels";

        public TaskLabelService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<ApiResponse<List<TaskLabelDTO>>> GetLabelsAsync()
        {
            try
            {
                var response = await _apiClient.GetAsync<ApiResponse<List<TaskLabelDTO>>>(BaseRoute);
                return response ?? new ApiResponse<List<TaskLabelDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TaskLabelDTO>>
                {
                    Success = false,
                    Errors = new[] { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<int>> CreateLabelAsync(CreateTaskLabelDTO command)
        {
            try
            {
                var response = await _apiClient.PostAsync<CreateTaskLabelDTO, ApiResponse<int>>(
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

        public async Task<ApiResponse<bool>> UpdateLabelAsync(UpdateTaskLabelDTO command)
        {
            try
            {
                var response = await _apiClient.PutAsync<UpdateTaskLabelDTO, ApiResponse<bool>>(
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

        public async Task<ApiResponse<object>> DeleteLabelAsync(int id)
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

        public async Task<ApiResponse<List<TaskLabelDTO>>> SearchLabelsAsync(TaskLabelFilterDTO filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var response = await _apiClient.GetAsync<ApiResponse<List<TaskLabelDTO>>>(
                    $"{BaseRoute}/search{query}");
                return response ?? new ApiResponse<List<TaskLabelDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TaskLabelDTO>>
                {
                    Success = false,
                    Errors = new[] { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<List<TaskLabelDTO>>> GetLabelsByUserAsync(int userId)
        {
            try
            {
                var response = await _apiClient.GetAsync<ApiResponse<List<TaskLabelDTO>>>(
                    $"{BaseRoute}/user/{userId}");
                return response ?? new ApiResponse<List<TaskLabelDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TaskLabelDTO>>
                {
                    Success = false,
                    Errors = new[] { ex.Message }
                };
            }
        }
    }
}
