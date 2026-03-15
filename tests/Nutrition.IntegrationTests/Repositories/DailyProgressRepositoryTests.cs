using FluentAssertions;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Filters;
using Nutrition.Domain.ValueObjects;
using Nutrition.Infrastructure.Persistence.Repositories;
using Nutrition.IntegrationTests.Fixtures;
using Nutrition.IntegrationTests.Helpers;

namespace Nutrition.IntegrationTests.Repositories
{
    [Trait("Category", "Integration")]
    [Trait("Layer", "Infrastructure")]
    public class DailyProgressRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private DailyProgressRepository _repository = null!;

        public DailyProgressRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            var context = _fixture.CreateNewContext();
            _repository = new DailyProgressRepository(context);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task Create_ShouldPersistDailyProgress()
        {
            var progress = TestDataFactory.CreateDailyProgress(userId: 1, caloriesConsumed: 500, liquidsConsumedMl: 1000);

            await _repository.Create(progress);

            var result = await _repository.GetById(progress.Id);
            result.Should().NotBeNull();
            result!.CaloriesConsumed.Should().Be(500);
            result.LiquidsConsumedMl.Should().Be(1000);
        }

        [Fact]
        public async Task GetById_WhenExists_ShouldReturnDailyProgress()
        {
            var progress = TestDataFactory.CreateDailyProgress(userId: 2);
            await _repository.Create(progress);

            var result = await _repository.GetById(progress.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(progress.Id);
        }

        [Fact]
        public async Task GetById_WhenNotExists_ShouldReturnNull()
        {
            var result = await _repository.GetById(99999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllDailyProgresses()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var progress1 = TestDataFactory.CreateDailyProgress(userId: 10, date: today, caloriesConsumed: 100, liquidsConsumedMl: 200);
            var progress2 = TestDataFactory.CreateDailyProgress(userId: 11, date: today, caloriesConsumed: 300, liquidsConsumedMl: 400);
            await _repository.Create(progress1);
            await _repository.Create(progress2);

            var result = await _repository.GetAll();

            result.Should().HaveCountGreaterThanOrEqualTo(2);
        }

        [Fact]
        public async Task GetAllByUserId_ShouldReturnOnlyUserRecords()
        {
            var userId = 60;
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var progress1 = TestDataFactory.CreateDailyProgress(userId: userId, date: today, caloriesConsumed: 100, liquidsConsumedMl: 200);
            var progress2 = TestDataFactory.CreateDailyProgress(userId: 61, date: today, caloriesConsumed: 300, liquidsConsumedMl: 400);
            await _repository.Create(progress1);
            await _repository.Create(progress2);

            var result = await _repository.GetAllByUserId(userId, CancellationToken.None);

            result.Should().HaveCount(1);
            result.First()!.UserId.Should().Be(userId);
        }

        [Fact]
        public async Task Update_ShouldModifyDailyProgress()
        {
            var progress = TestDataFactory.CreateDailyProgress(userId: 3, caloriesConsumed: 100, liquidsConsumedMl: 200);
            await _repository.Create(progress);

            var toUpdate = await _repository.GetById(progress.Id);
            toUpdate!.SetConsumed(999, 1500);
            await _repository.Update(toUpdate);

            using var readContext = _fixture.CreateNewContext();
            var readRepo = new DailyProgressRepository(readContext);
            var result = await readRepo.GetById(progress.Id);
            result.Should().NotBeNull();
            result!.CaloriesConsumed.Should().Be(999);
            result.LiquidsConsumedMl.Should().Be(1500);
        }

        [Fact]
        public async Task Delete_ShouldRemoveDailyProgress()
        {
            var progress = TestDataFactory.CreateDailyProgress(userId: 4, caloriesConsumed: 100, liquidsConsumedMl: 200);
            await _repository.Create(progress);

            var toDelete = await _repository.GetById(progress.Id);
            await _repository.Delete(toDelete!);

            using var readContext = _fixture.CreateNewContext();
            var readRepo = new DailyProgressRepository(readContext);
            var result = await readRepo.GetById(progress.Id);
            result.Should().BeNull();
        }

        [Fact]
        public async Task SetGoal_ShouldPersistGoal()
        {
            var progress = TestDataFactory.CreateDailyProgress(userId: 5, caloriesConsumed: 0, liquidsConsumedMl: 0);
            await _repository.Create(progress);

            var toUpdate = await _repository.GetById(progress.Id);
            toUpdate!.SetGoal(new DailyGoal(2000, 3000));
            await _repository.Update(toUpdate);

            using var readContext = _fixture.CreateNewContext();
            var readRepo = new DailyProgressRepository(readContext);
            var result = await readRepo.GetById(progress.Id);
            result.Should().NotBeNull();
            result!.Goal.Should().NotBeNull();
            result.Goal!.Calories.Should().Be(2000);
            result.Goal.QuantityMl.Should().Be(3000);
        }
    }
}
