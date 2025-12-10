using LifeSyncApp.Client.Models.Financial.Category;

namespace LifeSyncApp.Client.Models.Financial.Transaction
{
    public class TransactionDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public CategoryDTO? Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public TransactionType TransactionType { get; set; } = TransactionType.Expense;
        public Money Amount { get; set; } = new Money();
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; } = DateTime.Today;
        public bool IsRecurring { get; set; } = false;
    }
}