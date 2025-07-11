namespace Gym.Application.DTOs.Routine
{
    public record CreateRoutineDTO(
        string Name,
        string Description,
        int UserId);
}
