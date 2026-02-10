using LifeSyncApp.Models.Financial;

namespace LifeSyncApp.DTOs.Financial
{
    public class UpdateTransactionDTO
    {
        public int? CategoryId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public TransactionType TransactionType { get; set; }
        public Money Amount { get; set; } = new Money();
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
    }
}
