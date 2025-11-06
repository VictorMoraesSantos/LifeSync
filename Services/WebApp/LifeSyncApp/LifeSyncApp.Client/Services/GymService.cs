using LifeSyncApp.Client.Models;
using LifeSyncApp.Client.Services.Http;
using System.Net.Http.Json;
using LifeSyncApp.Client.Models.Gym;

namespace LifeSyncApp.Client.Services
{
    public interface IGymService
    {
        Task<ApiResponse<List<ExerciseDTO>>> GetExercisesAsync();
        Task<ApiResponse<int>> CreateExerciseAsync(CreateExerciseCommand command);
        Task<ApiResponse<bool>> UpdateExerciseAsync(UpdateExerciseCommand command);
        Task<ApiResponse<object>> DeleteExerciseAsync(int id);

        Task<ApiResponse<List<RoutineDTO>>> GetRoutinesAsync();
        Task<ApiResponse<int>> CreateRoutineAsync(CreateRoutineCommand command);
        Task<ApiResponse<bool>> UpdateRoutineAsync(UpdateRoutineCommand command);
        Task<ApiResponse<object>> DeleteRoutineAsync(int id);

        Task<ApiResponse<List<TrainingSessionDTO>>> GetTrainingSessionsAsync();
        Task<ApiResponse<int>> CreateTrainingSessionAsync(CreateTrainingSessionCommand command);
        Task<ApiResponse<bool>> UpdateTrainingSessionAsync(UpdateTrainingSessionCommand command);
        Task<ApiResponse<object>> DeleteTrainingSessionAsync(int id);

        Task<ApiResponse<List<ExerciseDTO>>> SearchExercisesAsync(object filter);
        Task<ApiResponse<List<RoutineDTO>>> SearchRoutinesAsync(object filter);
        Task<ApiResponse<List<TrainingSessionDTO>>> SearchTrainingSessionsAsync(object filter);
    }

    public class GymService : IGymService
    {
        private readonly IApiClient _apiClient;

        public GymService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<ApiResponse<List<ExerciseDTO>>> GetExercisesAsync()
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<ExerciseDTO>>>("gym-service/api/exercises");
                return res ?? new ApiResponse<List<ExerciseDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ExerciseDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<int>> CreateExerciseAsync(CreateExerciseCommand command)
        {
            try
            {
                var res = await _apiClient.PostAsync<CreateExerciseCommand, ApiResponse<int>>("gym-service/api/exercises", command);
                return res ?? new ApiResponse<int> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<bool>> UpdateExerciseAsync(UpdateExerciseCommand command)
        {
            try
            {
                var res = await _apiClient.PutAsync<UpdateExerciseCommand, ApiResponse<bool>>($"gym-service/api/exercises/{command.Id}", command);
                return res ?? new ApiResponse<bool> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<object>> DeleteExerciseAsync(int id)
        {
            try
            {
                await _apiClient.DeleteAsync($"gym-service/api/exercises/{id}");
                return new ApiResponse<object> { Success = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<RoutineDTO>>> GetRoutinesAsync()
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<RoutineDTO>>>("gym-service/api/routines");
                return res ?? new ApiResponse<List<RoutineDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<RoutineDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<int>> CreateRoutineAsync(CreateRoutineCommand command)
        {
            try
            {
                var res = await _apiClient.PostAsync<CreateRoutineCommand, ApiResponse<int>>("gym-service/api/routines", command);
                return res ?? new ApiResponse<int> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<bool>> UpdateRoutineAsync(UpdateRoutineCommand command)
        {
            try
            {
                var res = await _apiClient.PutAsync<UpdateRoutineCommand, ApiResponse<bool>>($"gym-service/api/routines/{command.Id}", command);
                return res ?? new ApiResponse<bool> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<object>> DeleteRoutineAsync(int id)
        {
            try
            {
                await _apiClient.DeleteAsync($"gym-service/api/routines/{id}");
                return new ApiResponse<object> { Success = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<TrainingSessionDTO>>> GetTrainingSessionsAsync()
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<TrainingSessionDTO>>>("gym-service/api/training-sessions");
                return res ?? new ApiResponse<List<TrainingSessionDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TrainingSessionDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<int>> CreateTrainingSessionAsync(CreateTrainingSessionCommand command)
        {
            try
            {
                var res = await _apiClient.PostAsync<CreateTrainingSessionCommand, ApiResponse<int>>("gym-service/api/training-sessions", command);
                return res ?? new ApiResponse<int> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<bool>> UpdateTrainingSessionAsync(UpdateTrainingSessionCommand command)
        {
            try
            {
                var res = await _apiClient.PutAsync<UpdateTrainingSessionCommand, ApiResponse<bool>>($"gym-service/api/training-sessions/{command.Id}", command);
                return res ?? new ApiResponse<bool> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<object>> DeleteTrainingSessionAsync(int id)
        {
            try
            {
                await _apiClient.DeleteAsync($"gym-service/api/training-sessions/{id}");
                return new ApiResponse<object> { Success = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<ExerciseDTO>>> SearchExercisesAsync(object filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var res = await _apiClient.GetAsync<ApiResponse<List<ExerciseDTO>>>($"gym-service/api/exercises/search{query}");
                return res ?? new ApiResponse<List<ExerciseDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ExerciseDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<RoutineDTO>>> SearchRoutinesAsync(object filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var res = await _apiClient.GetAsync<ApiResponse<List<RoutineDTO>>>($"gym-service/api/routines/search{query}");
                return res ?? new ApiResponse<List<RoutineDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<RoutineDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<TrainingSessionDTO>>> SearchTrainingSessionsAsync(object filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var res = await _apiClient.GetAsync<ApiResponse<List<TrainingSessionDTO>>>($"gym-service/api/training-sessions/search{query}");
                return res ?? new ApiResponse<List<TrainingSessionDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TrainingSessionDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }
    }
}
