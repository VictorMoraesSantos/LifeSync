using Bogus;
using Nutrition.Domain.Entities;
using Nutrition.Domain.ValueObjects;

namespace Nutrition.IntegrationTests.Helpers
{
    public static class TestDataFactory
    {
        private static readonly Faker _faker = new();

        public static Diary CreateDiary(int? userId = null, DateOnly? date = null)
        {
            return new Diary(
                userId ?? _faker.Random.Int(1, 1000),
                date ?? DateOnly.FromDateTime(DateTime.UtcNow));
        }

        public static Meal CreateMeal(string? name = null, string? description = null)
        {
            return new Meal(
                name ?? _faker.Lorem.Word(),
                description ?? _faker.Lorem.Sentence());
        }

        public static Food CreateFood(
            string? name = null,
            int? calories = null)
        {
            return new Food(
                name ?? _faker.Lorem.Word(),
                calories ?? _faker.Random.Int(10, 500),
                _faker.Random.Decimal(0, 50),
                _faker.Random.Decimal(0, 30),
                _faker.Random.Decimal(0, 80),
                _faker.Random.Decimal(0, 500),
                _faker.Random.Decimal(0, 300),
                _faker.Random.Decimal(0, 20),
                _faker.Random.Decimal(0, 2000),
                _faker.Random.Decimal(0, 1000));
        }

        public static MealFood CreateMealFood(int mealId, int foodId, int? quantity = null)
        {
            return new MealFood(
                mealId,
                foodId,
                quantity ?? _faker.Random.Int(50, 500));
        }

        public static LiquidType CreateLiquidType(string? name = null)
        {
            return new LiquidType(name ?? _faker.Lorem.Word());
        }

        public static Liquid CreateLiquid(int diaryId, int liquidTypeId, int? quantity = null)
        {
            return new Liquid(
                diaryId,
                liquidTypeId,
                quantity ?? _faker.Random.Int(100, 1000));
        }

        public static DailyProgress CreateDailyProgress(
            int? userId = null,
            DateOnly? date = null,
            int? caloriesConsumed = null,
            int? liquidsConsumedMl = null)
        {
            return new DailyProgress(
                userId ?? _faker.Random.Int(1, 1000),
                date ?? DateOnly.FromDateTime(DateTime.UtcNow),
                caloriesConsumed ?? _faker.Random.Int(0, 3000),
                liquidsConsumedMl ?? _faker.Random.Int(0, 5000));
        }

        public static DailyGoal CreateDailyGoal(int? calories = null, int? quantityMl = null)
        {
            return new DailyGoal(
                calories ?? _faker.Random.Int(1500, 3000),
                quantityMl ?? _faker.Random.Int(1500, 4000));
        }
    }
}
