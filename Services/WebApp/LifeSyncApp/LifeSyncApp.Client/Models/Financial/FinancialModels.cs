namespace LifeSyncApp.Client.Models.Financial
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public CategoryDto? Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int PaymentMethod { get; set; }
        public int TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "BRL";
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public bool IsRecurring { get; set; }
    }

    public class CreateTransactionRequest
    {
        public int UserId { get; set; }
        public int? CategoryId { get; set; }
        public int PaymentMethod { get; set; }
        public int TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "BRL";
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public bool IsRecurring { get; set; }
    }

    public class UpdateTransactionRequest
    {
        public int? CategoryId { get; set; }
        public int PaymentMethod { get; set; }
        public int TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class CreateCategoryRequest
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class UpdateCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public enum PaymentMethod
    {
        Cash = 1,
        CreditCard = 2,
        DebitCard = 3,
        BankTransfer = 4,
        DigitalWallet = 5
    }

    public enum TransactionType
    {
        Income = 1,
        Expense = 2
    }
}
