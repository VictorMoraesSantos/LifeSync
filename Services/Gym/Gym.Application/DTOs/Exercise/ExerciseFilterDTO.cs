using Core.Application.DTOs;
using Gym.Domain.Enums;

namespace Gym.Application.DTOs.Exercise
{
    public record ExerciseFilterDTO(
        int? Id,
        string? NameContains,
        string? DescriptionContains,
        MuscleGroup? MuscleGroupContains,
        ExerciseType? ExerciseTypeContains,
        EquipmentType? EquipmentTypeContains,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize) : DomainQueryFilterDTO(CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);
}
