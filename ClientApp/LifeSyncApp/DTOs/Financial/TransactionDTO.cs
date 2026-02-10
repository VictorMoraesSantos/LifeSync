using LifeSyncApp.Models.Financial;

namespace LifeSyncApp.DTOs.Financial
{
    public class TransactionDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public CategoryDTO? Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public TransactionType TransactionType { get; set; }
        public Money Amount { get; set; } = new Money();
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public bool IsRecurring { get; set; }
    }
}
