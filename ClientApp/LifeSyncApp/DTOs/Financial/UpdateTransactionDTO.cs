using LifeSyncApp.Models.Financial.Enums;
using System.Text.Json.Serialization;

namespace LifeSyncApp.DTOs.Financial;

public class UpdateTransactionDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("categoryId")]
    public int? CategoryId { get; set; }

    [JsonPropertyName("paymentMethod")]
    public PaymentMethod PaymentMethod { get; set; }

    [JsonPropertyName("transactionType")]
    public TransactionType TransactionType { get; set; }

    [JsonPropertyName("amount")]
    public MoneyDTO Amount { get; set; } = null!;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("transactionDate")]
    public DateTime TransactionDate { get; set; }

    [JsonPropertyName("isRecurring")]
    public bool IsRecurring { get; set; }
}
