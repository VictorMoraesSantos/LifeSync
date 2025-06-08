using Core.Domain.Entities;
using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;

namespace Financial.Domain.Entities
{
    public class Transaction : BaseEntity<int>
    {
        public int UserId { get; private set; }
        public int FinancialAccountId { get; private set; }
        public FinancialAccount FinancialAccount { get; private set; }
        public int? CategoryId { get; private set; }
        public Category? Category { get; private set; }
        public TransactionType Type { get; private set; }
        public Money Amount { get; private set; }
        public string Description { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public bool IsRecurring { get; private set; } = false;

        private Transaction() { }

        public Transaction(
            int userId,
            int financialAccountId,
            int? categoryId,
            TransactionType type,
            Money amount,
            string description,
            DateTime transactionDate,
            bool isRecurring = false)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(financialAccountId);
            ArgumentNullException.ThrowIfNull(amount);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(description);

            UserId = userId;
            FinancialAccountId = financialAccountId;
            CategoryId = categoryId;
            Type = type;
            Amount = amount;
            Description = description;
            TransactionDate = DateTime.SpecifyKind(transactionDate, DateTimeKind.Utc);
            IsRecurring = isRecurring;
        }

        public void Update(TransactionType type, Money amount, string description, DateTime transactionDate, int? categoryId, bool isRecurring)
        {
            ArgumentNullException.ThrowIfNull(amount);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(description);
            if (categoryId.HasValue) ArgumentOutOfRangeException.ThrowIfNegativeOrZero(categoryId.Value);

            Type = type;
            Amount = amount;
            Description = description;
            TransactionDate = DateTime.SpecifyKind(transactionDate, DateTimeKind.Utc);
            CategoryId = categoryId;
            IsRecurring = isRecurring;
            MarkAsUpdated();
        }
    }
}
