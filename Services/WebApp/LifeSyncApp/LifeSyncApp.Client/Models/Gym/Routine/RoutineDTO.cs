namespace LifeSyncApp.Client.Models.Gym.Routine
{
    public record RoutineDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        string Description);
}