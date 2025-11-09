namespace LifeSyncApp.Client.Models.Nutrition.Liquid
{
    public record CreateLiquidDTO(int DiaryId, string Name, int QuantityMl, int CaloriesPerMl);
}