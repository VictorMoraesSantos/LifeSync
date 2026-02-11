using LifeSyncApp.Models.Financial;

namespace LifeSyncApp.DTOs.Financial.Transaction
{
    public record TransactionFilterDTO(
        int? Id = null,
        int? UserId = null,
        int? CategoryId = null,
        PaymentMethod? PaymentMethod = null,
        TransactionType? TransactionType = null,
        int? AmountEquals = null,
        int? AmountGreaterThan = null,
        int? AmountLessThan = null,
        string? CurrencyEquals = null,
        string? DescriptionContains = null,
        DateOnly? TransactionDate = null,
        DateOnly? TransactionDateFrom = null,
        DateOnly? TransactionDateTo = null,
        DateOnly? CreatedAt = null,
        DateOnly? UpdatedAt = null,
        bool? IsDeleted = null,
        string? SortBy = null,
        bool? SortDesc = null,
        int? Page = null,
        int? PageSize = null);
}
