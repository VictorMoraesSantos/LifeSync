using Core.Domain.Entities;
using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;

namespace Financial.Domain.Entities
{
    public class FinancialAccount : BaseEntity<int>
    {
        public int UserId { get; private set; }
        public string Name { get; private set; }
        public string AccountType { get; private set; }
        public Money InitialBalance { get; private set; }
        public Currency Currency { get; private set; }

        private readonly List<Transaction> _transactions = new();
        public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

        private FinancialAccount() { }

        public FinancialAccount(int userId, string name, string accountType, Money initialBalance)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(accountType)) throw new ArgumentNullException(nameof(accountType));
            ArgumentNullException.ThrowIfNull(initialBalance);

            UserId = userId;
            Name = name;
            AccountType = accountType;
            InitialBalance = initialBalance;
            Currency = initialBalance.Currency;
        }

        public void UpdateDetails(string name, string accountType)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(accountType)) throw new ArgumentNullException(nameof(accountType));

            Name = name;
            AccountType = accountType;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
