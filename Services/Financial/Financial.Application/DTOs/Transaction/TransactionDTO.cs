using Core.Application.DTO;
using Financial.Application.DTOs.Category;
using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;

namespace Financial.Application.DTOs.Transaction
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
        bool IsRecurring = false)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
