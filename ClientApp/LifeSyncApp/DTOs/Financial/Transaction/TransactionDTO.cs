using LifeSyncApp.DTOs.Financial.Category;
using LifeSyncApp.Models.Financial;

namespace LifeSyncApp.DTOs.Financial.Transaction
{
    public record TransactionDTO(
        int Id,
        int UserId,
        CategoryDTO Category,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        PaymentMethod PaymentMethod,
        TransactionType TransactionType,
        Money Amount,
        string Description,
        DateTime TransactionDate,
        bool IsRecurring = false);
}
