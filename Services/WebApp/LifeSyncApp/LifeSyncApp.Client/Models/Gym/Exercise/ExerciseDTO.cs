namespace LifeSyncApp.Client.Models.Gym.Exercise
{
    public record ExerciseDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        string Description,
        MuscleGroup MuscleGroup,
        ExerciseType Type,
        EquipmentType? EquipmentType);
}