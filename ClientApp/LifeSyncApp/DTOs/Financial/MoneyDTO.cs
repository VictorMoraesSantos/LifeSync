using LifeSyncApp.Models.Financial.Enums;
using System.Text.Json.Serialization;

namespace LifeSyncApp.DTOs.Financial;

public class MoneyDTO
{
    [JsonPropertyName("amount")]
    public int Amount { get; set; }

    [JsonPropertyName("currency")]
    public Currency Currency { get; set; }

    public MoneyDTO() { }

    public MoneyDTO(int amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }
}
