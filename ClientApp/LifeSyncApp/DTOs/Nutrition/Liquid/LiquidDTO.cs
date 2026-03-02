namespace LifeSyncApp.DTOs.Nutrition.Liquid
{
    public record LiquidDTO(
        int Id,
        int DiaryId,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        int Quantity);
}
