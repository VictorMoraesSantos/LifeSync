using Core.Application.DTOs;

namespace Nutrition.Application.DTOs.DailyProgress
{
    public record DailyProgressQueryFilterDTO(
        int? Id,
        int? UserId,
        DateOnly? Date,
        int? CaloriesConsumedEqual,
        int? CaloriesConsumedGreaterThen,
        int? CaloriesConsumedLessThen,
        int? LiquidsConsumedMlEqual,
        int? LiquidsConsumedMlGreaterThen,
        int? LiquidsConsumedMlLessThen,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize)
        : DomainQueryFilterDTO(CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);
}
