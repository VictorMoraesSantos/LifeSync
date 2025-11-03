using LifeSyncApp.Client.Models.Common;
using LifeSyncApp.Client.Models.Gym;
using System.Net.Http.Json;

namespace LifeSyncApp.Client.Services
{
    public class GymService : IGymService
    {
        private readonly HttpClient _httpClient;

        public GymService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResult<List<ExerciseDto>>> GetExercisesAsync()
        {
            var response = await _httpClient.GetAsync("/gym-service/api/exercises");
            var result = await response.Content.ReadFromJsonAsync<HttpResult<List<ExerciseDto>>>();
            return result ?? new HttpResult<List<ExerciseDto>> { Success = false };
        }

        public async Task<HttpResult<ExerciseDto>> GetExerciseByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/gym-service/api/exercises/{id}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult<ExerciseDto>>();
            return result ?? new HttpResult<ExerciseDto> { Success = false };
        }

        public async Task<HttpResult<ExerciseDto>> CreateExerciseAsync(CreateExerciseRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/gym-service/api/exercises", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult<ExerciseDto>>();
            return result ?? new HttpResult<ExerciseDto> { Success = false };
        }

        public async Task<HttpResult> UpdateExerciseAsync(int id, UpdateExerciseRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"/gym-service/api/exercises/{id}", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { Success = false };
        }

        public async Task<HttpResult> DeleteExerciseAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/gym-service/api/exercises/{id}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { Success = false };
        }

        public async Task<HttpResult<List<RoutineDto>>> GetRoutinesAsync()
        {
            var response = await _httpClient.GetAsync("/gym-service/api/routines");
            var result = await response.Content.ReadFromJsonAsync<HttpResult<List<RoutineDto>>>();
            return result ?? new HttpResult<List<RoutineDto>> { Success = false };
        }

        public async Task<HttpResult<RoutineDto>> GetRoutineByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/gym-service/api/routines/{id}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult<RoutineDto>>();
            return result ?? new HttpResult<RoutineDto> { Success = false };
        }

        public async Task<HttpResult<RoutineDto>> CreateRoutineAsync(CreateRoutineRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/gym-service/api/routines", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult<RoutineDto>>();
            return result ?? new HttpResult<RoutineDto> { Success = false };
        }

        public async Task<HttpResult> UpdateRoutineAsync(int id, UpdateRoutineRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"/gym-service/api/routines/{id}", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { Success = false };
        }

        public async Task<HttpResult> DeleteRoutineAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/gym-service/api/routines/{id}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { Success = false };
        }

        public async Task<HttpResult<List<TrainingSessionDto>>> GetTrainingSessionsAsync()
        {
            var response = await _httpClient.GetAsync("/gym-service/api/training-sessions");
            var result = await response.Content.ReadFromJsonAsync<HttpResult<List<TrainingSessionDto>>>();
            return result ?? new HttpResult<List<TrainingSessionDto>> { Success = false };
        }

        public async Task<HttpResult<TrainingSessionDto>> GetTrainingSessionByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/gym-service/api/training-sessions/{id}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult<TrainingSessionDto>>();
            return result ?? new HttpResult<TrainingSessionDto> { Success = false };
        }

        public async Task<HttpResult<TrainingSessionDto>> CreateTrainingSessionAsync(CreateTrainingSessionRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/gym-service/api/training-sessions", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult<TrainingSessionDto>>();
            return result ?? new HttpResult<TrainingSessionDto> { Success = false };
        }

        public async Task<HttpResult> UpdateTrainingSessionAsync(int id, UpdateTrainingSessionRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"/gym-service/api/training-sessions/{id}", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { Success = false };
        }

        public async Task<HttpResult> DeleteTrainingSessionAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/gym-service/api/training-sessions/{id}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { Success = false };
        }
    }
}
