using LifeSyncApp.Models.Financial.Enums;

namespace LifeSyncApp.DTOs.Financial;

public class TransactionFilterDTO
{
    public int? UserId { get; set; }
    public int? CategoryId { get; set; }
    public TransactionType? TransactionType { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = true;
}
