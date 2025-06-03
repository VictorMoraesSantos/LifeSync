using Core.Application.DTO;
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
        Money Balance)
        : DTOBase(Id, CreatedAt, UpdatedAt)
    {

    };
}
