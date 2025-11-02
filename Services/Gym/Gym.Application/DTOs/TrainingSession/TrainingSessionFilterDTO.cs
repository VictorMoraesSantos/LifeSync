using Core.Application.DTOs;

namespace Gym.Application.DTOs.TrainingSession
{
    public record TrainingSessionFilterDTO(
        int? Id,
        int? UserId,
        int? RoutineId,
        DateOnly? StartTime,
        DateOnly? EndTime,
        string? NotesContains,
        int? CompletedExerciseId,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize) : DomainQueryFilterDTO(CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);
}
