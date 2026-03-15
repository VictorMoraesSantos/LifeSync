using FluentAssertions;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Filters;
using Nutrition.Infrastructure.Persistence.Repositories;
using Nutrition.IntegrationTests.Fixtures;
using Nutrition.IntegrationTests.Helpers;

namespace Nutrition.IntegrationTests.Repositories
{
    [Trait("Category", "Integration")]
    [Trait("Layer", "Infrastructure")]
    public class DiaryRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private DiaryRepository _repository = null!;

        public DiaryRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            var context = _fixture.CreateNewContext();
            _repository = new DiaryRepository(context);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task Create_ShouldPersistDiary()
        {
            var diary = TestDataFactory.CreateDiary(userId: 1);

            await _repository.Create(diary);

            var result = await _repository.GetById(diary.Id);
            result.Should().NotBeNull();
            result!.UserId.Should().Be(1);
        }

        [Fact]
        public async Task GetById_WhenExists_ShouldReturnDiary()
        {
            var diary = TestDataFactory.CreateDiary(userId: 2);
            await _repository.Create(diary);

            var result = await _repository.GetById(diary.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(diary.Id);
        }

        [Fact]
        public async Task GetById_WhenNotExists_ShouldReturnNull()
        {
            var result = await _repository.GetById(99999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllDiaries()
        {
            var diary1 = TestDataFactory.CreateDiary(userId: 10);
            var diary2 = TestDataFactory.CreateDiary(userId: 11);
            await _repository.Create(diary1);
            await _repository.Create(diary2);

            var result = await _repository.GetAll();

            result.Should().HaveCountGreaterThanOrEqualTo(2);
        }

        [Fact]
        public async Task GetAllByUserId_ShouldReturnOnlyUserDiaries()
        {
            var userId = 50;
            var diary1 = TestDataFactory.CreateDiary(userId: userId);
            var diary2 = TestDataFactory.CreateDiary(userId: userId);
            var diary3 = TestDataFactory.CreateDiary(userId: 51);
            await _repository.Create(diary1);
            await _repository.Create(diary2);
            await _repository.Create(diary3);

            var result = await _repository.GetAllByUserId(userId);

            result.Should().HaveCount(2);
            result.Should().AllSatisfy(d => d.UserId.Should().Be(userId));
        }

        [Fact]
        public async Task Update_ShouldModifyDiary()
        {
            var diary = TestDataFactory.CreateDiary(userId: 3);
            await _repository.Create(diary);

            var newDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
            diary.UpdateDate(newDate);

            using var updateContext = _fixture.CreateNewContext();
            var updateRepo = new DiaryRepository(updateContext);
            await updateRepo.Update(diary);

            using var readContext = _fixture.CreateNewContext();
            var readRepo = new DiaryRepository(readContext);
            var result = await readRepo.GetById(diary.Id);
            result.Should().NotBeNull();
            result!.Date.Should().Be(newDate);
        }

        [Fact]
        public async Task Delete_ShouldRemoveDiary()
        {
            var diary = TestDataFactory.CreateDiary(userId: 4);
            await _repository.Create(diary);

            using var deleteContext = _fixture.CreateNewContext();
            var deleteRepo = new DiaryRepository(deleteContext);
            var toDelete = await deleteRepo.GetById(diary.Id);
            await deleteRepo.Delete(toDelete!);

            using var readContext = _fixture.CreateNewContext();
            var readRepo = new DiaryRepository(readContext);
            var result = await readRepo.GetById(diary.Id);
            result.Should().BeNull();
        }

        [Fact]
        public async Task FindByFilter_ShouldReturnFilteredResults()
        {
            var userId = 100;
            var diary = TestDataFactory.CreateDiary(userId: userId);
            await _repository.Create(diary);

            var filter = new DiaryQueryFilter(
                id: null,
                userId: userId,
                totalCaloriesEquals: null,
                totalCaloriesGreaterThan: null,
                totalCaloriesLessThan: null,
                totalLiquidsMlEquals: null,
                totalLiquidsMlGreaterThan: null,
                totalLiquidsMlLessThan: null,
                mealId: null,
                liquidId: null,
                createdAt: null,
                updatedAt: null,
                isDeleted: null,
                sortBy: null,
                sortDesc: null,
                page: 1,
                pageSize: 10);

            var (items, totalCount) = await _repository.FindByFilter(filter);

            totalCount.Should().BeGreaterThanOrEqualTo(1);
            items.Should().NotBeEmpty();
        }
    }
}
