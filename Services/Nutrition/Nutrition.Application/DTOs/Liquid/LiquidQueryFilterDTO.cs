using Core.Application.DTOs;

namespace Nutrition.Application.DTOs.Liquid
{
    public record LiquidQueryFilterDTO(
        int? Id,
        string? NameContains,
        int? QuantityMlEquals,
        int? QuantityMlGreaterThan,
        int? QuantityMlLessThan,
        int? CaloriesPerMlEquals,
        int? CaloriesPerMlGreaterThan,
        int? CaloriesPerMlLessThan,
        int? DiaryId,
        int? TotalCaloriesEquals,
        int? TotalCaloriesGreaterThan,
        int? TotalCaloriesLessThan,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize
    ) : DomainQueryFilterDTO(CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);

}
