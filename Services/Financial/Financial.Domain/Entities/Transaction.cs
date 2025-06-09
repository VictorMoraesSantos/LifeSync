using Core.Domain.Entities;
using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;

namespace Financial.Domain.Entities
{
    public class Transaction : BaseEntity<int>
    {
        public int UserId { get; private set; }
        public int? CategoryId { get; private set; }
        public Category? Category { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public TransactionType TransactionType { get; private set; }
        public Money Amount { get; private set; }
        public string Description { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public bool IsRecurring { get; private set; } = false;

        private Transaction() { }

        public Transaction(
            int userId,
            int? categoryId,
            PaymentMethod paymentMethod,
            TransactionType transactionType,
            Money amount,
            string description,
            DateTime transactionDate,
            bool isRecurring = false)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);
            ArgumentNullException.ThrowIfNull(amount);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(description);

            UserId = userId;
            CategoryId = categoryId;
            PaymentMethod = paymentMethod;
            TransactionType = transactionType;
            Amount = amount;
            Description = description;
            TransactionDate = DateTime.SpecifyKind(transactionDate, DateTimeKind.Utc);
            IsRecurring = isRecurring;
        }

        public void Update(
            int? categoryId,
            PaymentMethod paymentMethod,
            TransactionType transactionType,
            Money amount,
            string description,
            DateTime transactionDate,
            bool isRecurring = false)
        {
            ArgumentNullException.ThrowIfNull(amount);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(description);
            if (categoryId.HasValue) ArgumentOutOfRangeException.ThrowIfNegativeOrZero(categoryId.Value);

            CategoryId = categoryId;
            PaymentMethod = paymentMethod;
            TransactionType = transactionType;
            Amount = amount;
            Description = description;
            TransactionDate = DateTime.SpecifyKind(transactionDate, DateTimeKind.Utc);
            IsRecurring = isRecurring;
            MarkAsUpdated();
        }
    }
}
