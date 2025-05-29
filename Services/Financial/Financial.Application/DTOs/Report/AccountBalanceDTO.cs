using Financial.Domain.Enums;

namespace Financial.Application.DTOs.Report
{
    public record AccountBalanceDTO(
        int AccountId,
        string AccountName,
        int Balance,
        Currency Currency);
}
