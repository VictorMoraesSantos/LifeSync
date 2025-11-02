using Core.Application.DTOs;

namespace Gym.Application.DTOs.Routine
{
    public record RoutineFilterDTO(
        int? Id,
        string? NameContains,
        string? DescriptionContains,
        int? RoutineExerciseId,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize) : DomainQueryFilterDTO(CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);
}
