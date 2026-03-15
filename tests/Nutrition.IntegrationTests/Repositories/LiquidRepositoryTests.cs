using FluentAssertions;
using Nutrition.Domain.Entities;
using Nutrition.Infrastructure.Persistence.Repositories;
using Nutrition.IntegrationTests.Fixtures;
using Nutrition.IntegrationTests.Helpers;

namespace Nutrition.IntegrationTests.Repositories
{
    [Trait("Category", "Integration")]
    [Trait("Layer", "Infrastructure")]
    public class LiquidRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private LiquidRepository _repository = null!;
        private DiaryRepository _diaryRepository = null!;
        private LiquidTypeRepository _liquidTypeRepository = null!;

        public LiquidRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            var context = _fixture.CreateNewContext();
            _repository = new LiquidRepository(context);
            _diaryRepository = new DiaryRepository(context);
            _liquidTypeRepository = new LiquidTypeRepository(context);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private async Task<(Diary diary, LiquidType liquidType)> CreatePrerequisitesAsync()
        {
            var diary = TestDataFactory.CreateDiary(userId: 1);
            await _diaryRepository.Create(diary);
            var liquidType = TestDataFactory.CreateLiquidType("Water");
            await _liquidTypeRepository.Create(liquidType);
            return (diary, liquidType);
        }

        [Fact]
        public async Task Create_ShouldPersistLiquid()
        {
            var (diary, liquidType) = await CreatePrerequisitesAsync();
            var liquid = TestDataFactory.CreateLiquid(diary.Id, liquidType.Id, 500);

            await _repository.Create(liquid);

            var result = await _repository.GetById(liquid.Id);
            result.Should().NotBeNull();
            result!.Quantity.Should().Be(500);
        }

        [Fact]
        public async Task GetById_WhenExists_ShouldReturnLiquid()
        {
            var (diary, liquidType) = await CreatePrerequisitesAsync();
            var liquid = TestDataFactory.CreateLiquid(diary.Id, liquidType.Id);
            await _repository.Create(liquid);

            var result = await _repository.GetById(liquid.Id);

            result.Should().NotBeNull();
            result!.DiaryId.Should().Be(diary.Id);
        }

        [Fact]
        public async Task GetById_WhenNotExists_ShouldReturnNull()
        {
            var result = await _repository.GetById(99999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllLiquids()
        {
            var (diary, liquidType) = await CreatePrerequisitesAsync();
            var liquid1 = TestDataFactory.CreateLiquid(diary.Id, liquidType.Id, 200);
            var liquid2 = TestDataFactory.CreateLiquid(diary.Id, liquidType.Id, 300);
            await _repository.Create(liquid1);
            await _repository.Create(liquid2);

            var result = await _repository.GetAll();

            result.Should().HaveCountGreaterThanOrEqualTo(2);
        }

        [Fact]
        public async Task Update_ShouldModifyLiquid()
        {
            var (diary, liquidType) = await CreatePrerequisitesAsync();
            var liquid = TestDataFactory.CreateLiquid(diary.Id, liquidType.Id, 200);
            await _repository.Create(liquid);

            using var updateContext = _fixture.CreateNewContext();
            var updateRepo = new LiquidRepository(updateContext);
            var toUpdate = await updateRepo.GetById(liquid.Id);
            toUpdate!.Update(liquidType.Id, 750);
            await updateRepo.Update(toUpdate);

            using var readContext = _fixture.CreateNewContext();
            var readRepo = new LiquidRepository(readContext);
            var result = await readRepo.GetById(liquid.Id);
            result.Should().NotBeNull();
            result!.Quantity.Should().Be(750);
        }

        [Fact]
        public async Task Delete_ShouldRemoveLiquid()
        {
            var (diary, liquidType) = await CreatePrerequisitesAsync();
            var liquid = TestDataFactory.CreateLiquid(diary.Id, liquidType.Id);
            await _repository.Create(liquid);

            using var deleteContext = _fixture.CreateNewContext();
            var deleteRepo = new LiquidRepository(deleteContext);
            var toDelete = await deleteRepo.GetById(liquid.Id);
            await deleteRepo.Delete(toDelete!);

            using var readContext = _fixture.CreateNewContext();
            var readRepo = new LiquidRepository(readContext);
            var result = await readRepo.GetById(liquid.Id);
            result.Should().BeNull();
        }
    }
}
