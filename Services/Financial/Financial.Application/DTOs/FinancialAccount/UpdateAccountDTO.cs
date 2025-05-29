using Financial.Domain.Enums;

namespace Financial.Application.DTOs.FinancialAccount
{
    public record UpdateAccountDTO(
        string Id,
        string Name,
        string AccountType,
        Currency Currency);
}
