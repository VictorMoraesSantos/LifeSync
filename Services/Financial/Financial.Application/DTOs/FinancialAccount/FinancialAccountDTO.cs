using Core.Application.DTO;
using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;

namespace Financial.Application.DTOs.FinancialAccount
{
    public record FinancialAccountDTO(
        int Id,
        int UserId,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        string AccountType,
        Money Balance,
        Currency Currency)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
