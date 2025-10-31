using Core.Application.DTOs;

namespace Nutrition.Application.DTOs.Diary
{
    public record DiaryFilterQueryDTO(
        int? Id,
        int? UserId,
        int? TotalCaloriesEqual,
        int? TotalCaloriesGreaterThen,
        int? TotalCaloriesLessThen,
        int? TotalLiquidsMlEqual,
        int? TotalLiquidsMlGreaterThen,
        int? TotalLiquidsMlLessThen,
        int? MealId,
        int? LiquidId,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize
    ) : DomainQueryFilterDTO(CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);
}
