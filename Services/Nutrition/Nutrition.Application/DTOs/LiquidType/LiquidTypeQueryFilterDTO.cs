using Core.Application.DTOs;

namespace Nutrition.Application.DTOs.LiquidType
{
    public record LiquidTypeQueryFilterDTO(
        int? Id,
        string? NameContains,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize)
        : DomainQueryFilterDTO(CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);
}
