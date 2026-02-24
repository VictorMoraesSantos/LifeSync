using CsvHelper.Configuration;

namespace Nutrition.Infrastructure.DataSeeders.Csv
{
    public class FoodMap : ClassMap<FoodCsvRow>
    {
        public FoodMap()
        {
            Map(m => m.Name).Name("Name");
            Map(m => m.Calories).Name("Calories");
            Map(m => m.Protein).Name("Protein");
            Map(m => m.Lipids).Name("Lipids");
            Map(m => m.Carbohydrates).Name("Carbohydrates");
            Map(m => m.Calcium).Name("Calcium");
            Map(m => m.Magnesium).Name("Magnesium");
            Map(m => m.Iron).Name("Iron");
            Map(m => m.Sodium).Name("Sodium");
            Map(m => m.Potassium).Name("Potassium");
        }
    }
}
