using Core.Domain.Filters;
using Financial.Domain.Entities;
using System.Linq.Expressions;

namespace Financial.Domain.Filters.Specifications
{
    public class TransactionSpecification : BaseFilterSpecification<Transaction, int>
    {
        public TransactionSpecification(TransactionQueryFilter filter)
        : base(filter, BuildCriteria, ConfigureIncludes)
        { }

        private static Expression<Func<Transaction, bool>>? BuildCriteria(IDomainQueryFilter baseFilter)
        {
            var filter = (TransactionQueryFilter)baseFilter;
            var builder = new FilterCriteriaBuilder<Transaction, int>(filter)
            .AddCommonFilters()
            .AddIf(filter.Id.HasValue, t => t.Id == filter.Id!.Value)
            .AddIf(filter.UserId.HasValue, t => t.UserId == filter.UserId!.Value)
            .AddIf(filter.CategoryId.HasValue, t => t.CategoryId == filter.CategoryId!.Value)
            .AddIf(filter.PaymentMethod.HasValue, t => t.PaymentMethod == filter.PaymentMethod!.Value)
            .AddIf(filter.TransactionType.HasValue, t => t.TransactionType == filter.TransactionType!.Value)
            .AddIf(filter.AmountEquals.HasValue, t => t.Amount.Amount == filter.AmountEquals!.Value)
            .AddIf(filter.AmountGreaterThan.HasValue, t => t.Amount.Amount > filter.AmountGreaterThan!.Value)
            .AddIf(filter.AmountLessThan.HasValue, t => t.Amount.Amount < filter.AmountLessThan!.Value)
            .AddIf(!string.IsNullOrWhiteSpace(filter.CurrencyEquals), t => t.Amount.Currency.ToString() == filter.CurrencyEquals)
            .AddIf(!string.IsNullOrWhiteSpace(filter.DescriptionContains), t => t.Description.Contains(filter.DescriptionContains!))
            .AddIf(filter.TransactionDate.HasValue, t => DateOnly.FromDateTime(t.TransactionDate) == filter.TransactionDate!.Value)
            .AddIf(filter.TransactionDateFrom.HasValue, t => DateOnly.FromDateTime(t.TransactionDate) >= filter.TransactionDateFrom!.Value)
            .AddIf(filter.TransactionDateTo.HasValue, t => DateOnly.FromDateTime(t.TransactionDate) <= filter.TransactionDateTo!.Value);

            return builder.Build();
        }

        private static void ConfigureIncludes(BaseFilterSpecification<Transaction, int> spec)
        {
            spec.AddInclude(t => t.Category);
        }
    }
}
