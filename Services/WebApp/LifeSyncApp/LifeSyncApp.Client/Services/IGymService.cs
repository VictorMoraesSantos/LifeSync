using LifeSyncApp.Client.Models.Common;
using LifeSyncApp.Client.Models.Gym;

namespace LifeSyncApp.Client.Services
{
    public interface IGymService
    {
        Task<HttpResult<List<ExerciseDto>>> GetExercisesAsync();
        Task<HttpResult<ExerciseDto>> GetExerciseByIdAsync(int id);
        Task<HttpResult<ExerciseDto>> CreateExerciseAsync(CreateExerciseRequest request);
        Task<HttpResult> UpdateExerciseAsync(int id, UpdateExerciseRequest request);
        Task<HttpResult> DeleteExerciseAsync(int id);

        Task<HttpResult<List<RoutineDto>>> GetRoutinesAsync();
        Task<HttpResult<RoutineDto>> GetRoutineByIdAsync(int id);
        Task<HttpResult<RoutineDto>> CreateRoutineAsync(CreateRoutineRequest request);
        Task<HttpResult> UpdateRoutineAsync(int id, UpdateRoutineRequest request);
        Task<HttpResult> DeleteRoutineAsync(int id);

        Task<HttpResult<List<TrainingSessionDto>>> GetTrainingSessionsAsync();
        Task<HttpResult<TrainingSessionDto>> GetTrainingSessionByIdAsync(int id);
        Task<HttpResult<TrainingSessionDto>> CreateTrainingSessionAsync(CreateTrainingSessionRequest request);
        Task<HttpResult> UpdateTrainingSessionAsync(int id, UpdateTrainingSessionRequest request);
        Task<HttpResult> DeleteTrainingSessionAsync(int id);
    }
}
