using FluentAssertions;
using Nutrition.Domain.Entities;
using Nutrition.Infrastructure.Persistence.Repositories;
using Nutrition.IntegrationTests.Fixtures;
using Nutrition.IntegrationTests.Helpers;

namespace Nutrition.IntegrationTests.Repositories
{
    [Trait("Category", "Integration")]
    [Trait("Layer", "Infrastructure")]
    public class MealRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private MealRepository _repository = null!;
        private DiaryRepository _diaryRepository = null!;

        public MealRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            var context = _fixture.CreateNewContext();
            _repository = new MealRepository(context);
            _diaryRepository = new DiaryRepository(context);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private async Task<Diary> CreateDiaryAsync()
        {
            var diary = TestDataFactory.CreateDiary(userId: 1);
            await _diaryRepository.Create(diary);
            return diary;
        }

        [Fact]
        public async Task Create_ShouldPersistMeal()
        {
            var diary = await CreateDiaryAsync();
            var meal = TestDataFactory.CreateMeal();
            meal.SetDiaryId(diary.Id);

            await _repository.Create(meal);

            var result = await _repository.GetById(meal.Id);
            result.Should().NotBeNull();
            result!.Name.Should().Be(meal.Name);
        }

        [Fact]
        public async Task GetById_WhenExists_ShouldReturnMeal()
        {
            var diary = await CreateDiaryAsync();
            var meal = TestDataFactory.CreateMeal(name: "Lunch");
            meal.SetDiaryId(diary.Id);
            await _repository.Create(meal);

            var result = await _repository.GetById(meal.Id);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Lunch");
        }

        [Fact]
        public async Task GetById_WhenNotExists_ShouldReturnNull()
        {
            var result = await _repository.GetById(99999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllMeals()
        {
            var diary = await CreateDiaryAsync();
            var meal1 = TestDataFactory.CreateMeal(name: "Breakfast");
            meal1.SetDiaryId(diary.Id);
            var meal2 = TestDataFactory.CreateMeal(name: "Dinner");
            meal2.SetDiaryId(diary.Id);
            await _repository.Create(meal1);
            await _repository.Create(meal2);

            var result = await _repository.GetAll();

            result.Should().HaveCountGreaterThanOrEqualTo(2);
        }

        [Fact]
        public async Task Update_ShouldModifyMeal()
        {
            var diary = await CreateDiaryAsync();
            var meal = TestDataFactory.CreateMeal(name: "OldName");
            meal.SetDiaryId(diary.Id);
            await _repository.Create(meal);

            var mealToUpdate = await _repository.GetById(meal.Id);
            mealToUpdate!.Update("NewName", "NewDescription");
            await _repository.Update(mealToUpdate);

            using var readContext = _fixture.CreateNewContext();
            var readRepo = new MealRepository(readContext);
            var result = await readRepo.GetById(meal.Id);
            result.Should().NotBeNull();
            result!.Name.Should().Be("NewName");
        }

        [Fact]
        public async Task Delete_ShouldRemoveMeal()
        {
            var diary = await CreateDiaryAsync();
            var meal = TestDataFactory.CreateMeal();
            meal.SetDiaryId(diary.Id);
            await _repository.Create(meal);

            var mealToDelete = await _repository.GetById(meal.Id);
            await _repository.Delete(mealToDelete!);

            using var readContext = _fixture.CreateNewContext();
            var readRepo = new MealRepository(readContext);
            var result = await readRepo.GetById(meal.Id);
            result.Should().BeNull();
        }
    }
}
