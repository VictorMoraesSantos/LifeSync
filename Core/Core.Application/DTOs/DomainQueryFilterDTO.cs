namespace Core.Application.DTOs
{
    public record DomainQueryFilterDTO<TId>(
        TId? Id,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize);
}
