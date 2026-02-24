using Core.Application.DTOs;

namespace Nutrition.Application.DTOs.Liquid
{
    public record LiquidQueryFilterDTO(
        int? Id,
        int? DiaryId,
        string? NameContains,
        int? QuantityEquals,
        int? QuantityGreaterThan,
        int? QuantityLessThan,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize)
        : DomainQueryFilterDTO(CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);

}
