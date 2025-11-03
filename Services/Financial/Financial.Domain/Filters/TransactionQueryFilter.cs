using Core.Domain.Filters;
using Financial.Domain.Enums;

namespace Financial.Domain.Filters
{
    public class TransactionQueryFilter : DomainQueryFilter
    {
        public int? Id { get; private set; }
        public int? UserId { get; private set; }
        public int? CategoryId { get; private set; }
        public PaymentMethod? PaymentMethod { get; private set; }
        public TransactionType? TransactionType { get; private set; }
        public int? AmountEquals { get; private set; }
        public int? AmountGreaterThan { get; private set; }
        public int? AmountLessThan { get; private set; }
        public string? CurrencyEquals { get; private set; }
        public string? DescriptionContains { get; private set; }
        public DateOnly? TransactionDate { get; private set; }
        public DateOnly? TransactionDateFrom { get; private set; }
        public DateOnly? TransactionDateTo { get; private set; }

        public TransactionQueryFilter(
        int? id = null,
        int? userId = null,
        int? categoryId = null,
        PaymentMethod? paymentMethod = null,
        TransactionType? transactionType = null,
        int? amountEquals = null,
        int? amountGreaterThan = null,
        int? amountLessThan = null,
        string? currencyEquals = null,
        string? descriptionContains = null,
        DateOnly? transactionDate = null,
        DateOnly? transactionDateFrom = null,
        DateOnly? transactionDateTo = null,
        DateOnly? createdAt = null,
        DateOnly? updatedAt = null,
        bool? isDeleted = null,
        string? sortBy = null,
        bool? sortDesc = null,
        int? page = null,
        int? pageSize = null)
        {
            Id = id;
            UserId = userId;
            CategoryId = categoryId;
            PaymentMethod = paymentMethod;
            TransactionType = transactionType;
            AmountEquals = amountEquals;
            AmountGreaterThan = amountGreaterThan;
            AmountLessThan = amountLessThan;
            CurrencyEquals = currencyEquals;
            DescriptionContains = descriptionContains;
            TransactionDate = transactionDate;
            TransactionDateFrom = transactionDateFrom;
            TransactionDateTo = transactionDateTo;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsDeleted = isDeleted;
            SortBy = sortBy;
            SortDesc = sortDesc;
            Page = page;
            PageSize = pageSize;
        }
    }
}
