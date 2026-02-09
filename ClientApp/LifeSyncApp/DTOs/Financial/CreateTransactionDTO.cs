using LifeSyncApp.Models.Financial.Enums;

namespace LifeSyncApp.DTOs.Financial;

public class CreateTransactionDTO
{
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BRL";
    public string Description { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public bool IsRecurring { get; set; }
}
