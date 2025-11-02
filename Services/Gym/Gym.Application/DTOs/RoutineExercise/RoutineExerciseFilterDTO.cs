using Core.Application.DTOs;

namespace Gym.Application.DTOs.RoutineExercise
{
    public record RoutineExerciseFilterDTO(
        int? Id,
        int? RoutineId,
        int? ExerciseId,
        int? SetsEquals,
        int? SetsLessThan,
        int? SetsGreaterThan,
        int? RepetitionsEquals,
        int? RepetitionsLessThan,
        int? RepetitionsGreaterThan,
        int? RestTimeEquals,
        int? RestTimeLessThan,
        int? RestTimeGreaterThan,
        int? RecommendedWeightEquals,
        int? RecommendedWeightLessThan,
        int? RecommendedWeightGreaterThan,
        string? InstructionsContains,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize) : DomainQueryFilterDTO(CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);
}
