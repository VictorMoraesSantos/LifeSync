using Core.Application.DTOs;

namespace Nutrition.Application.DTOs.DailyProgress
{
    public record DailyProgressQueryFilterDTO(
        int? Id,
        int? UserId,
        DateOnly? Date,
        int? CaloriesConsumedEquals,
        int? CaloriesConsumedGreaterThan,
        int? CaloriesConsumedLessThan,
        int? LiquidsConsumedMlEquals,
        int? LiquidsConsumedMlGreaterThan,
        int? LiquidsConsumedMlLessThan,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize)
        : DomainQueryFilterDTO(CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);
}
