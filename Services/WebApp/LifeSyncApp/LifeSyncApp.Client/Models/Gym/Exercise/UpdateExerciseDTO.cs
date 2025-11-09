namespace LifeSyncApp.Client.Models.Gym.Exercise
{
    public record UpdateExerciseDTO(
        int Id,
        string Name,
        string Description,
        MuscleGroup MuscleGroup,
        ExerciseType ExerciseType,
        EquipmentType? EquipmentType);
}