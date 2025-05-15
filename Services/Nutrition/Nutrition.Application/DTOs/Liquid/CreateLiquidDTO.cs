namespace Nutrition.Application.DTOs.Liquid
{
    public record CreateLiquidDTO(int DiaryId, string Name, int QuantityMl, int CaloriesPerMl);
}
