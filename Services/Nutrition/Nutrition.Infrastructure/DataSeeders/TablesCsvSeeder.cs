using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nutrition.Application.DTOs.MealFood;
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
            if(await _context.MealFoods.AnyAsync()) return;

            var csvPath = Path.Combine(AppContext.BaseDirectory, "DataSeeders", "Csv", "CsvFiles", "MealFood.csv");

            if(!File.Exists(csvPath)) return;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HeaderValidated = null,
                MissingFieldFound = null,
                PrepareHeaderForMatch = args => args.Header.ToLower(),
                TrimOptions = TrimOptions.Trim,           
                WhiteSpaceChars = new[] { ' ' },
            };

            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, config);

            csv.Context.RegisterClassMap<MealFoodMap>();

            var dtos = csv.GetRecords<MealFoodCsvDTO>().ToList();

            var records = dtos.Select(dto => new MealFood(
                code: dto.Code,
                name: dto.Name,
                calories: dto.Calories,
                protein: dto.Protein,
                lipids: dto.Lipids,
                carbohydrates: dto.Carbohydrates,
                calcium: dto.Calcium,
                magnesium: dto.Magnesium,
                iron: dto.Iron,
                sodium: dto.Sodium,
                potassium: dto.Potassium,
                quantity: dto.Quantity
            )).ToList();

            await _context.MealFoods.AddRangeAsync(records);
            await _context.SaveChangesAsync();
        }
    }
}
