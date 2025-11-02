using Core.Application.DTOs;

namespace Gym.Application.DTOs.CompletedExercise
{
    public record CompletedExerciseFilterDTO(
        int? Id,
        int? TrainingSessionId,
        int? RoutineExerciseId,
        int? SetsCompletedEquals,
        int? SetsCompletedLessThan,
        int? SetsCompletedGreaterThan,
        int? RepetitionsCompletedEquals,
        int? RepetitionsCompletedLessThan,
        int? RepetitionsCompletedGreaterThan,
        int? WeightUsedCompletedEquals,
        int? WeightUsedCompletedLessThan,
        int? WeightUsedCompletedGreaterThan,
        DateOnly? CompletedAt,
        string? NotesContains,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize) : DomainQueryFilterDTO(CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);
}
