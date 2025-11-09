using LifeSyncApp.Client.Models;
using LifeSyncApp.Client.Models.Gym;

namespace LifeSyncApp.Client.Services.Contracts
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

        Task<ApiResponse<List<RoutineExerciseDTO>>> GetRoutineExercisesAsync(int routineId);
        Task<ApiResponse<int>> CreateRoutineExerciseAsync(CreateRoutineExerciseCommand command);
        Task<ApiResponse<bool>> UpdateRoutineExerciseAsync(UpdateRoutineExerciseCommand command);
        Task<ApiResponse<object>> DeleteRoutineExerciseAsync(int id);

        Task<ApiResponse<List<TrainingSessionDTO>>> GetTrainingSessionsAsync();
        Task<ApiResponse<int>> CreateTrainingSessionAsync(CreateTrainingSessionCommand command);
        Task<ApiResponse<bool>> UpdateTrainingSessionAsync(UpdateTrainingSessionCommand command);
        Task<ApiResponse<object>> DeleteTrainingSessionAsync(int id);

        Task<ApiResponse<int>> CreateCompletedExerciseAsync(CreateCompletedExerciseCommand command);
        Task<ApiResponse<bool>> UpdateCompletedExerciseAsync(UpdateCompletedExerciseCommand command);
        Task<ApiResponse<List<CompletedExerciseDTO>>> GetCompletedExercisesBySessionAsync(int trainingSessionId);
        Task<ApiResponse<object>> DeleteCompletedExerciseAsync(int id);

        Task<ApiResponse<List<ExerciseDTO>>> SearchExercisesAsync(object filter);
        Task<ApiResponse<List<RoutineDTO>>> SearchRoutinesAsync(object filter);
        Task<ApiResponse<List<TrainingSessionDTO>>> SearchTrainingSessionsAsync(object filter);
    }
}
