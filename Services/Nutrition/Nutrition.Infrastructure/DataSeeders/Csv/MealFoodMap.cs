using CsvHelper.Configuration;

namespace Nutrition.Infrastructure.DataSeeders.Csv
{
    public class MealFoodMap : ClassMap<MealFoodCsvDTO>
    {
        public MealFoodMap()
        {
            Map(m => m.Code).Name("code");
            Map(m => m.Name).Name("name");
            Map(m => m.Calories).Name("calories");
            Map(m => m.Protein).Name("protein");
            Map(m => m.Lipids).Name("lipids");
            Map(m => m.Carbohydrates).Name("carbohydrates");
            Map(m => m.Calcium).Name("calcium");
            Map(m => m.Magnesium).Name("magnesium");
            Map(m => m.Iron).Name("iron");
            Map(m => m.Sodium).Name("sodium");
            Map(m => m.Potassium).Name("potassium");
            Map(m => m.Quantity).Name("quantity");
        }
    }
}
