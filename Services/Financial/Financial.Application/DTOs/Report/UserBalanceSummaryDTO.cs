using Financial.Domain.Enums;

namespace Financial.Application.DTOs.Report
{
    public record UserBalanceSummaryDTO(
        int TotalBalance,
        Currency Currency,
        IEnumerable<AccountBalanceDTO> AccountBalances);
}
