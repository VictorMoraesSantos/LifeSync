using Core.Application.DTO;
using Financial.Domain.Enums;

namespace Financial.Application.DTOs.Transaction
{
    public record TransactionDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        int FinancialAccountId,
        TransactionType Type,
        int Amount,
        string Currency,
        string Description,
        DateTime TransactionDate,
        int? CategoryId,
        bool IsRecurring = false)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
