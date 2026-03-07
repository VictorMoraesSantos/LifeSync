using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Nutrition.Domain.Entities;
using Nutrition.Infrastructure.DataSeeders.Csv;
using Nutrition.Infrastructure.Persistence.Data;
using System.Globalization;

namespace Nutrition.Infrastructure.DataSeeders
{
    public class TablesCsvSeeder
    {
        private readonly ApplicationDbContext _context;

        public TablesCsvSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await SeedMealFoodsAsync();
        }

        private async Task SeedMealFoodsAsync()
        {
            if (await _context.Foods.AnyAsync()) return;

            var csvPath = Path.Combine(AppContext.BaseDirectory, "DataSeeders", "Csv", "CsvFiles", "Food.csv");

            if (!File.Exists(csvPath)) return;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HeaderValidated = null,
                MissingFieldFound = null,
                TrimOptions = TrimOptions.Trim,
                WhiteSpaceChars = new[] { ' ' },
            };

            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, config);

            csv.Context.RegisterClassMap<FoodMap>();
            var rows = csv.GetRecords<FoodCsvRow>().ToList();

            var foods = rows.Select(r => new Food(
                r.Name,
                r.Calories,
                r.Protein,
                r.Lipids,
                r.Carbohydrates,
                r.Calcium,
                r.Magnesium,
                r.Iron,
                r.Sodium,
                r.Potassium)).ToList();

            await _context.Foods.AddRangeAsync(foods);
            await _context.SaveChangesAsync();
        }
    }
}
