using Core.Application.DTOs;

namespace Gym.Application.DTOs.Exercise
{
    public record ExerciseFilterDTO(
        int? Id,
        string? NameContains,
        string? DescriptionContains,
        string? MuscleGroupContains,
        string? ExerciseTypeContains,
        string? EquipmentTypeContains,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize) : DomainQueryFilterDTO(CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);
}
