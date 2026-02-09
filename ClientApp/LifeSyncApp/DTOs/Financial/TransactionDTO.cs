using LifeSyncApp.Models.Financial.Enums;
using System.Text.Json.Serialization;

namespace LifeSyncApp.DTOs.Financial;

public class TransactionDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("category")]
    public CategoryDTO Category { get; set; } = null!;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime? UpdatedAt { get; set; }

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
