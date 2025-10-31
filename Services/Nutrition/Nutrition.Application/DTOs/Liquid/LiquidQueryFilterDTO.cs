using Core.Application.DTOs;

namespace Nutrition.Application.DTOs.Liquid
{
    public record LiquidQueryFilterDTO(
        int? Id,
        string? NameContains,
        int? QuantityMlEqual,
        int? QuantityMlGreaterThen,
        int? QuantityMlLessThen,
        int? CaloriesPerMlEqual,
        int? CaloriesPerMlGreaterThen,
        int? CaloriesPerMlLessThen,
        int? DiaryId,
        int? TotalCaloriesEqual,
        int? TotalCaloriesGreaterThen,
        int? TotalCaloriesLessThen,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize
    ) : DomainQueryFilterDTO(CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);

}
