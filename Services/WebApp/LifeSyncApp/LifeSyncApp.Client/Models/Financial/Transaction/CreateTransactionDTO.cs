namespace LifeSyncApp.Client.Models.Financial.Transaction
{
    public class CreateTransactionDTO
    {
        public int UserId { get; set; }
        public int? CategoryId { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public TransactionType TransactionType { get; set; } = TransactionType.Expense;
        public Money Amount { get; set; } = new Money();
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; } = DateTime.Today;
        public bool IsRecurring { get; set; } = false;
    }
}