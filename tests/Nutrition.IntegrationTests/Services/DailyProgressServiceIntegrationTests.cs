using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Nutrition.Application.DTOs.DailyProgress;
using Nutrition.Infrastructure.Persistence.Repositories;
using Nutrition.Infrastructure.Services;
using Nutrition.IntegrationTests.Fixtures;
using Nutrition.IntegrationTests.Helpers;

namespace Nutrition.IntegrationTests.Services
{
    [Trait("Category", "Integration")]
    [Trait("Layer", "Infrastructure")]
    public class DailyProgressServiceIntegrationTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private DailyProgressService _service = null!;
        private DailyProgressRepository _repository = null!;

        public DailyProgressServiceIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            var context = _fixture.CreateNewContext();
            _repository = new DailyProgressRepository(context);
            var logger = NullLogger<DailyProgressService>.Instance;
            _service = new DailyProgressService(_repository, logger);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task CreateAsync_ShouldCreateDailyProgress()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var dto = new CreateDailyProgressDTO(1, today, 500, 1000);

            var result = await _service.CreateAsync(dto);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ShouldReturnProgress()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var createResult = await _service.CreateAsync(new CreateDailyProgressDTO(2, today, 200, 400));

            var result = await _service.GetByIdAsync(createResult.Value);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.UserId.Should().Be(2);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotExists_ShouldFail()
        {
            var result = await _service.GetByIdAsync(99999);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProgresses()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            await _service.CreateAsync(new CreateDailyProgressDTO(10, today, 100, 200));
            await _service.CreateAsync(new CreateDailyProgressDTO(11, today, 300, 400));

            var result = await _service.GetAllAsync();

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCountGreaterThanOrEqualTo(2);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnOnlyUserRecords()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var userId = 30;
            await _service.CreateAsync(new CreateDailyProgressDTO(userId, today, 100, 200));
            await _service.CreateAsync(new CreateDailyProgressDTO(31, today, 300, 400));

            var result = await _service.GetByUserIdAsync(userId, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProgress()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var createResult = await _service.CreateAsync(new CreateDailyProgressDTO(3, today, 100, 200));

            var result = await _service.UpdateAsync(new UpdateDailyProgressDTO(createResult.Value, 999, 1500));

            result.IsSuccess.Should().BeTrue();

            var getResult = await _service.GetByIdAsync(createResult.Value);
            getResult.Value!.CaloriesConsumed.Should().Be(999);
        }

        [Fact]
        public async Task SetGoalAsync_ShouldSetGoal()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var createResult = await _service.CreateAsync(new CreateDailyProgressDTO(4, today, 0, 0));

            var goalDto = new DailyGoalDTO(2000, 3000);
            var result = await _service.SetGoalAsync(createResult.Value, goalDto, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();

            var getResult = await _service.GetByIdAsync(createResult.Value);
            getResult.Value!.Goal.Should().NotBeNull();
            getResult.Value!.Goal.Calories.Should().Be(2000);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteProgress()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var createResult = await _service.CreateAsync(new CreateDailyProgressDTO(5, today, 100, 200));

            var result = await _service.DeleteAsync(createResult.Value);

            result.IsSuccess.Should().BeTrue();

            var getResult = await _service.GetByIdAsync(createResult.Value);
            getResult.IsSuccess.Should().BeFalse();
        }
    }
}
