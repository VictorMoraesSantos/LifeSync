using Core.Application.DTOs;
using Financial.Domain.Enums;

namespace Financial.Application.DTOs.Transaction
{
    public record TransactionFilterDTO(
        int? Id,
        int? UserId,
        int? CategoryId,
        PaymentMethod? PaymentMethod,
        TransactionType? TransactionType,
        int? AmountEquals,
        int? AmountGreaterThan,
        int? AmountLessThan,
        string? CurrencyEquals,
        string? DescriptionContains,
        DateOnly? TransactionDate,
        DateOnly? TransactionDateFrom,
        DateOnly? TransactionDateTo,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize)
        : DomainQueryFilterDTO(CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);
}
