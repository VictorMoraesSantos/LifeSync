namespace Gym.Application.DTOs.Routine
{
    public record CreateRoutineDTO(
        int UserId,
        string Name,
        string Description);
}
