namespace LifeSyncApp.Client.Models.Financial;

public enum PaymentMethod { Cash = 1, CreditCard, DebitCard, BankTransfer, Pix, Other }
public enum TransactionType { Income = 1, Expense = 2 }
public enum Currency { USD, EUR, BRL, GBP, JPY, CNY, AUD, CAD, CHF, INR }

public record Money(int Amount, Currency Currency);

// Read DTOs
public record CategoryDTO(
    int Id,
    int UserId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string Name,
    string? Description);

public record TransactionDTO(
    int Id,
    int UserId,
    CategoryDTO? Category,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    PaymentMethod PaymentMethod,
    TransactionType TransactionType,
    Money Amount,
    string Description,
    DateTime TransactionDate,
    bool IsRecurring = false);

// Commands
public record CreateTransactionCommand(
    int UserId,
    int? CategoryId,
    PaymentMethod PaymentMethod,
    TransactionType TransactionType,
    Money Amount,
    string Description,
    DateTime TransactionDate,
    bool IsRecurring = false);

public record UpdateTransactionCommand(
    int Id,
    int? CategoryId,
    PaymentMethod PaymentMethod,
    TransactionType TransactionType,
    Money Amount,
    string Description,
    DateTime TransactionDate,
    bool IsRecurring = false);

public record CreateCategoryCommand(int UserId, string Name, string? Description);
public record UpdateCategoryCommand(int Id, string Name, string? Description);
