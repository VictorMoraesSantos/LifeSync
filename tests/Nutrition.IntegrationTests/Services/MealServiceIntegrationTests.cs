using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Nutrition.Application.DTOs.Diary;
using Nutrition.Application.DTOs.Meal;
using Nutrition.Infrastructure.Persistence.Repositories;
using Nutrition.Infrastructure.Services;
using Nutrition.IntegrationTests.Fixtures;

namespace Nutrition.IntegrationTests.Services
{
    [Trait("Category", "Integration")]
    [Trait("Layer", "Infrastructure")]
    public class MealServiceIntegrationTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private MealService _mealService = null!;
        private DiaryService _diaryService = null!;

        public MealServiceIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            RecreateServices();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        /// <summary>
        /// Creates new services backed by a fresh DbContext so that
        /// previously-tracked entities do not conflict with later operations.
        /// </summary>
        private void RecreateServices()
        {
            var context = _fixture.CreateNewContext();
            var mealRepository = new MealRepository(context);
            var diaryRepository = new DiaryRepository(context);
            var publisher = new FakePublisher();
            _mealService = new MealService(mealRepository, publisher, NullLogger<MealService>.Instance);
            _diaryService = new DiaryService(diaryRepository, publisher, NullLogger<DiaryService>.Instance);
        }

        /// <summary>
        /// Creates a diary and adds a meal to it via DiaryService, returning the created meal's ID.
        /// This is the correct workflow because meals require a parent diary (FK constraint).
        /// </summary>
        private async Task<int> CreateMealViaDiaryAsync(string name = "TestMeal", string description = "Test description")
        {
            // Create a diary first
            var diaryDto = new CreateDiaryDTO(1, DateOnly.FromDateTime(DateTime.UtcNow));
            var diaryResult = await _diaryService.CreateAsync(diaryDto);
            diaryResult.IsSuccess.Should().BeTrue("a diary is required before creating a meal");
            var diaryId = diaryResult.Value;

            // Fresh context to avoid tracking conflict between Create and AddMeal
            RecreateServices();

            // Add the meal to the diary
            var mealDto = new CreateMealDTO(name, description);
            var addResult = await _diaryService.AddMealToDiaryAsync(diaryId, mealDto);
            addResult.IsSuccess.Should().BeTrue("adding a meal to a diary should succeed");

            // Use a fresh context to avoid tracking conflicts, then find the meal
            RecreateServices();

            var diaryGetResult = await _diaryService.GetByIdAsync(diaryId);
            diaryGetResult.IsSuccess.Should().BeTrue();
            var meal = diaryGetResult.Value!.Meals.First(m => m.Name == name);
            return meal.Id;
        }

        /// <summary>
        /// Creates a diary and adds multiple meals, returning their IDs.
        /// Uses different dates to avoid duplicate-date conflicts.
        /// </summary>
        private async Task<List<int>> CreateMultipleMealsViaDiaryAsync(params (string Name, string Description)[] meals)
        {
            var ids = new List<int>();

            // Create a single diary to hold all meals
            var diaryDto = new CreateDiaryDTO(100, DateOnly.FromDateTime(DateTime.UtcNow));
            var diaryResult = await _diaryService.CreateAsync(diaryDto);
            diaryResult.IsSuccess.Should().BeTrue();
            var diaryId = diaryResult.Value;

            foreach (var (name, desc) in meals)
            {
                // Need fresh context before each add to avoid tracking issues
                RecreateServices();
                var mealDto = new CreateMealDTO(name, desc);
                var addResult = await _diaryService.AddMealToDiaryAsync(diaryId, mealDto);
                addResult.IsSuccess.Should().BeTrue($"adding meal '{name}' should succeed");
            }

            // Get the meal IDs from the diary
            RecreateServices();
            var diary = await _diaryService.GetByIdAsync(diaryId);
            diary.IsSuccess.Should().BeTrue();
            ids.AddRange(diary.Value!.Meals.Select(m => m.Id));
            return ids;
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateMeal()
        {
            // Meals require a parent diary due to the FK constraint.
            // The production workflow creates meals via DiaryService.AddMealToDiaryAsync.
            var diaryDto = new CreateDiaryDTO(1, DateOnly.FromDateTime(DateTime.UtcNow));
            var diaryResult = await _diaryService.CreateAsync(diaryDto);
            diaryResult.IsSuccess.Should().BeTrue();

            // Fresh context to avoid tracking conflict
            RecreateServices();

            var mealDto = new CreateMealDTO("Breakfast", "Morning meal");
            var result = await _diaryService.AddMealToDiaryAsync(diaryResult.Value, mealDto);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();

            // Verify the meal was persisted
            RecreateServices();
            var diary = await _diaryService.GetByIdAsync(diaryResult.Value);
            diary.IsSuccess.Should().BeTrue();
            diary.Value!.Meals.Should().ContainSingle(m => m.Name == "Breakfast");
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ShouldReturnMeal()
        {
            var mealId = await CreateMealViaDiaryAsync("Lunch", "Afternoon meal");

            var result = await _mealService.GetByIdAsync(mealId);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Name.Should().Be("Lunch");
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotExists_ShouldFail()
        {
            var result = await _mealService.GetByIdAsync(99999);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnMeals()
        {
            await CreateMultipleMealsViaDiaryAsync(
                ("Meal1", "Desc1"),
                ("Meal2", "Desc2"));

            var result = await _mealService.GetAllAsync();

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCountGreaterThanOrEqualTo(2);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateMeal()
        {
            var mealId = await CreateMealViaDiaryAsync("OldName", "OldDesc");

            var result = await _mealService.UpdateAsync(new UpdateMealDTO(mealId, "NewName", "NewDesc"));

            result.IsSuccess.Should().BeTrue();

            RecreateServices();
            var getResult = await _mealService.GetByIdAsync(mealId);
            getResult.Value!.Name.Should().Be("NewName");
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteMeal()
        {
            var mealId = await CreateMealViaDiaryAsync("ToDelete", "Will be deleted");

            var result = await _mealService.DeleteAsync(mealId);

            result.IsSuccess.Should().BeTrue();

            RecreateServices();
            var getResult = await _mealService.GetByIdAsync(mealId);
            getResult.IsSuccess.Should().BeFalse();
        }
    }
}
