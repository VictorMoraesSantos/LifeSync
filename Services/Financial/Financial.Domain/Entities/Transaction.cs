using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Financial.Domain.Enums;
using Financial.Domain.Errors;
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
            SetAmout(amount);
            SetDescription(description);

            if (categoryId.HasValue)
                throw new DomainException(TransactionErrors.InvalidCategoryId);

            CategoryId = categoryId;
            PaymentMethod = paymentMethod;
            TransactionType = transactionType;
            Amount = amount;
            Description = description;
            TransactionDate = DateTime.SpecifyKind(transactionDate, DateTimeKind.Utc);
            IsRecurring = isRecurring;
            MarkAsUpdated();
        }

        private void SetAmout(Money amount)
        {
            if (amount == null)
                throw new DomainException(TransactionErrors.InvalidAmount);
            Amount = amount;
        }

        private void SetDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException(TransactionErrors.InvalidDescription);
            Description = description.Trim();
        }
    }
}
