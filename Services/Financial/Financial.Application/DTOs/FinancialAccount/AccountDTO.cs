using Core.Application.DTO;
using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;

namespace Financial.Application.DTOs.FinancialAccount
{
    public record AccountDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        string AccountType,
        Money Balance,
        Currency Currency)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
