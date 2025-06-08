using FinancialControl.Domain.ValueObjects;

namespace Financial.Application.DTOs.FinancialAccount
{
    public record CreateFinancialAccountDTO(
        int UserId,
        string Name,
        string AccountType,
        Money Balance);
}
