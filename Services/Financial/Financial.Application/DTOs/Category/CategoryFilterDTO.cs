using Core.Application.DTOs;

namespace Financial.Application.DTOs.Category
{
    public record CategoryFilterDTO(
    int? Id,
    int? UserId,
    string? NameContains,
    string? DescriptionContains,
    DateOnly? CreatedAt,
    DateOnly? UpdatedAt,
    bool? IsDeleted,
    string? SortBy,
    bool? SortDesc,
    int? Page,
    int? PageSize)
    : DomainQueryFilterDTO(CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);
}
