using Financial.Domain.Enums;

namespace Financial.Application.DTOs.FinancialAccount
{
    public record UpdateFinancialAccountDTO(
        int Id,
        string Name,
        string AccountType);
}
