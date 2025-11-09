namespace LifeSyncApp.Client.Models.Gym.Exercise
{
    public record CreateExerciseDTO(
        string Name,
        string Description,
        MuscleGroup MuscleGroup,
        ExerciseType ExerciseType,
        EquipmentType? EquipmentType);
}