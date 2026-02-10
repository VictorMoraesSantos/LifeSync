using LifeSyncApp.Models.Financial;

namespace LifeSyncApp.DTOs.Financial
{
    public class TransactionFilterDTO
    {
        public int? UserId { get; set; }
        public int? CategoryId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public TransactionType? TransactionType { get; set; }
        public DateTime? TransactionDateFrom { get; set; }
        public DateTime? TransactionDateTo { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
