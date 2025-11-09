using LifeSyncApp.Client.Models;
using LifeSyncApp.Client.Models.Gym;
using LifeSyncApp.Client.Services.Contracts;
using LifeSyncApp.Client.Services.Http;

namespace LifeSyncApp.Client.Services
{
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

        // Routine Exercises
        public async Task<ApiResponse<List<RoutineExerciseDTO>>> GetRoutineExercisesAsync(int routineId)
        {
            try
            {
                var query = $"?RoutineId={routineId}";
                var res = await _apiClient.GetAsync<ApiResponse<List<RoutineExerciseDTO>>>("gym-service/api/routine-exercises/search" + query);
                return res ?? new ApiResponse<List<RoutineExerciseDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<RoutineExerciseDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<int>> CreateRoutineExerciseAsync(CreateRoutineExerciseCommand command)
        {
            try
            {
                var res = await _apiClient.PostAsync<CreateRoutineExerciseCommand, ApiResponse<int>>("gym-service/api/routine-exercises", command);
                return res ?? new ApiResponse<int> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<bool>> UpdateRoutineExerciseAsync(UpdateRoutineExerciseCommand command)
        {
            try
            {
                var res = await _apiClient.PutAsync<UpdateRoutineExerciseCommand, ApiResponse<bool>>($"gym-service/api/routine-exercises/{command.Id}", command);
                return res ?? new ApiResponse<bool> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<object>> DeleteRoutineExerciseAsync(int id)
        {
            try
            {
                await _apiClient.DeleteAsync($"gym-service/api/routine-exercises/{id}");
                return new ApiResponse<object> { Success = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<int>> CreateCompletedExerciseAsync(CreateCompletedExerciseCommand command)
        {
            try
            {
                var res = await _apiClient.PostAsync<CreateCompletedExerciseCommand, ApiResponse<int>>("gym-service/api/completed-exercises", command);
                return res ?? new ApiResponse<int> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<bool>> UpdateCompletedExerciseAsync(UpdateCompletedExerciseCommand command)
        {
            try
            {
                var res = await _apiClient.PutAsync<UpdateCompletedExerciseCommand, ApiResponse<bool>>($"gym-service/api/completed-exercises/{command.Id}", command);
                return res ?? new ApiResponse<bool> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<CompletedExerciseDTO>>> GetCompletedExercisesBySessionAsync(int trainingSessionId)
        {
            try
            {
                var query = $"?TrainingSessionId={trainingSessionId}";
                var res = await _apiClient.GetAsync<ApiResponse<List<CompletedExerciseDTO>>>("gym-service/api/completed-exercises/search" + query);
                return res ?? new ApiResponse<List<CompletedExerciseDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<CompletedExerciseDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<object>> DeleteCompletedExerciseAsync(int id)
        {
            try
            {
                await _apiClient.DeleteAsync($"gym-service/api/completed-exercises/{id}");
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
