using LifeSyncApp.Client.Models;
using LifeSyncApp.Client.Services.Http;
using System.Net.Http.Json;

namespace LifeSyncApp.Client.Services
{
    // Gym Models
    public class Exercise
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string MuscleGroup { get; set; } = string.Empty;
        public string Equipment { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    public class Routine
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<RoutineExercise> Exercises { get; set; } = new();
    }

    public class RoutineExercise
    {
        public Guid ExerciseId { get; set; }
        public string ExerciseName { get; set; } = string.Empty;
        public int Sets { get; set; }
        public int Reps { get; set; }
        public decimal? Weight { get; set; }
        public int? RestTime { get; set; }
    }

    public class TrainingSession
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public Guid RoutineId { get; set; }
        public string RoutineName { get; set; } = string.Empty;
        public int Duration { get; set; }
        public List<CompletedExercise> CompletedExercises { get; set; } = new();
    }

    public class CompletedExercise
    {
        public Guid ExerciseId { get; set; }
        public string ExerciseName { get; set; } = string.Empty;
        public int SetsCompleted { get; set; }
        public int RepsCompleted { get; set; }
        public decimal? WeightUsed { get; set; }
    }

    // Lightweight filter models matching API
    public class ExerciseFilter
    {
        public int? Id { get; set; }
        public string? NameContains { get; set; }
        public string? DescriptionContains { get; set; }
        public string? MuscleGroupContains { get; set; }
        public string? ExerciseTypeContains { get; set; }
        public string? EquipmentTypeContains { get; set; }
        public DateOnly? CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
        public bool? IsDeleted { get; set; }
        public string? SortBy { get; set; }
        public bool? SortDesc { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

    public class RoutineFilter
    {
        public int? Id { get; set; }
        public string? NameContains { get; set; }
        public string? DescriptionContains { get; set; }
        public int? RoutineExerciseId { get; set; }
        public DateOnly? CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
        public bool? IsDeleted { get; set; }
        public string? SortBy { get; set; }
        public bool? SortDesc { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

    public class TrainingSessionFilter
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public int? RoutineId { get; set; }
        public DateOnly? StartTime { get; set; }
        public DateOnly? EndTime { get; set; }
        public string? NotesContains { get; set; }
        public int? CompletedExerciseId { get; set; }
        public DateOnly? CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
        public bool? IsDeleted { get; set; }
        public string? SortBy { get; set; }
        public bool? SortDesc { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

    // Gym Service
    public interface IGymService
    {
        Task<ApiResponse<List<Exercise>>> GetExercisesAsync();
        Task<ApiResponse<Exercise>> CreateExerciseAsync(Exercise exercise);
        Task<ApiResponse<List<Routine>>> GetRoutinesAsync();
        Task<ApiResponse<Routine>> CreateRoutineAsync(Routine routine);
        Task<ApiResponse<List<TrainingSession>>> GetTrainingSessionsAsync();
        Task<ApiResponse<TrainingSession>> CreateTrainingSessionAsync(TrainingSession session);

        // New search endpoints
        Task<ApiResponse<List<Exercise>>> SearchExercisesAsync(ExerciseFilter filter);
        Task<ApiResponse<List<Routine>>> SearchRoutinesAsync(RoutineFilter filter);
        Task<ApiResponse<List<TrainingSession>>> SearchTrainingSessionsAsync(TrainingSessionFilter filter);
    }

    public class GymService : IGymService
    {
        private readonly IApiClient _apiClient;

        public GymService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<ApiResponse<List<Exercise>>> GetExercisesAsync()
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<Exercise>>>("gym-service/api/exercises");
                return res ?? new ApiResponse<List<Exercise>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Exercise>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<Exercise>> CreateExerciseAsync(Exercise exercise)
        {
            try
            {
                var res = await _apiClient.PostAsync<Exercise, ApiResponse<Exercise>>("gym-service/api/exercises", exercise);
                return res ?? new ApiResponse<Exercise> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Exercise> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<Routine>>> GetRoutinesAsync()
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<Routine>>>("gym-service/api/routines");
                return res ?? new ApiResponse<List<Routine>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Routine>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<Routine>> CreateRoutineAsync(Routine routine)
        {
            try
            {
                var res = await _apiClient.PostAsync<Routine, ApiResponse<Routine>>("gym-service/api/routines", routine);
                return res ?? new ApiResponse<Routine> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Routine> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<TrainingSession>>> GetTrainingSessionsAsync()
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<TrainingSession>>>("gym-service/api/training-sessions");
                return res ?? new ApiResponse<List<TrainingSession>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TrainingSession>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<TrainingSession>> CreateTrainingSessionAsync(TrainingSession session)
        {
            try
            {
                var res = await _apiClient.PostAsync<TrainingSession, ApiResponse<TrainingSession>>("gym-service/api/training-sessions", session);
                return res ?? new ApiResponse<TrainingSession> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<TrainingSession> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<Exercise>>> SearchExercisesAsync(ExerciseFilter filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var res = await _apiClient.GetAsync<ApiResponse<List<Exercise>>>($"gym-service/api/exercises/search{query}");
                return res ?? new ApiResponse<List<Exercise>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Exercise>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<Routine>>> SearchRoutinesAsync(RoutineFilter filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var res = await _apiClient.GetAsync<ApiResponse<List<Routine>>>($"gym-service/api/routines/search{query}");
                return res ?? new ApiResponse<List<Routine>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Routine>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<TrainingSession>>> SearchTrainingSessionsAsync(TrainingSessionFilter filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var res = await _apiClient.GetAsync<ApiResponse<List<TrainingSession>>>($"gym-service/api/training-sessions/search{query}");
                return res ?? new ApiResponse<List<TrainingSession>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TrainingSession>> { Success = false, Errors = new[] { ex.Message } };
            }
        }
    }
}
