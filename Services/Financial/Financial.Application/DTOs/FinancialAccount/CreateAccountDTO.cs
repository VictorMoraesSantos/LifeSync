using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;

namespace Financial.Application.DTOs.FinancialAccount
{
    public record CreateAccountDTO(
        int UserId,
        string Name,
        string AccountType,
        Money Balance,
        Currency Currency);
}
