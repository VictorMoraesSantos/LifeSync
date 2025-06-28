using Core.Domain.Repositories;
using Gym.Domain.Entities;
using Gym.Domain.Enums;

namespace Gym.Domain.Repositories
{
    public interface IExerciseRepository : IRepository<Exercise, int>
    {
        Task<IEnumerable<Exercise>> GetByMuscleGroupAsync(MuscleGroup muscleGroup);
        Task<IEnumerable<Exercise>> GetByDifficultyLevelAsync(DifficultyLevel level);
        Task<IEnumerable<Exercise>> GetByExerciseTypeAsync(ExerciseType type);
        Task<IEnumerable<Exercise>> GetByEquipmentTypeAsync(EquipmentType equipmentType);
    }
}
