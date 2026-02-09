using LifeSyncApp.Models.Financial;
using LifeSyncApp.Models.Financial.Enums;

namespace LifeSyncApp.DTOs.Financial;

public class TransactionDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public CategoryDTO? Category { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public TransactionType TransactionType { get; set; }
    public MoneyDTO? Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public bool IsRecurring { get; set; }
}

public class MoneyDTO
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BRL";
}

public class CategoryDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Color { get; set; } = "#6366F1";
    public string Icon { get; set; } = "📋";
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
