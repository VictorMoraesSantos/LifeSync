using LifeSyncApp.Models.Financial.Category;
using LifeSyncApp.Models.Financial.RecurrenceSchedule;
using LifeSyncApp.Models.Financial;

namespace LifeSyncApp.Models.Financial.Transaction
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
        bool IsRecurring = false,
        RecurrenceScheduleInfoDTO? RecurrenceSchedule = null);
}
