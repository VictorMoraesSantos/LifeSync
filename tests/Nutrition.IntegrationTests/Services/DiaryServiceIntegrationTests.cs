using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Nutrition.Application.DTOs.Diary;
using Nutrition.Infrastructure.Persistence.Repositories;
using Nutrition.Infrastructure.Services;
using Nutrition.IntegrationTests.Fixtures;

namespace Nutrition.IntegrationTests.Services
{
    [Trait("Category", "Integration")]
    [Trait("Layer", "Infrastructure")]
    public class DiaryServiceIntegrationTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private DiaryService _service = null!;
        private DiaryRepository _repository = null!;

        public DiaryServiceIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            RecreateService();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        /// <summary>
        /// Creates a new service backed by a fresh DbContext so that
        /// previously-tracked entities do not conflict with later operations.
        /// </summary>
        private void RecreateService()
        {
            var context = _fixture.CreateNewContext();
            _repository = new DiaryRepository(context);
            var publisher = new FakePublisher();
            var logger = NullLogger<DiaryService>.Instance;
            _service = new DiaryService(_repository, publisher, logger);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateDiary()
        {
            var dto = new CreateDiaryDTO(1, DateOnly.FromDateTime(DateTime.UtcNow));

            var result = await _service.CreateAsync(dto);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CreateAsync_DuplicateDate_ShouldFail()
        {
            var date = DateOnly.FromDateTime(DateTime.UtcNow);
            var dto = new CreateDiaryDTO(1, date);
            await _service.CreateAsync(dto);

            var result = await _service.CreateAsync(dto);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ShouldReturnDiary()
        {
            var dto = new CreateDiaryDTO(2, DateOnly.FromDateTime(DateTime.UtcNow));
            var createResult = await _service.CreateAsync(dto);

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
        public async Task GetAllAsync_ShouldReturnDiaries()
        {
            await _service.CreateAsync(new CreateDiaryDTO(10, DateOnly.FromDateTime(DateTime.UtcNow)));
            await _service.CreateAsync(new CreateDiaryDTO(11, DateOnly.FromDateTime(DateTime.UtcNow)));

            var result = await _service.GetAllAsync();

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCountGreaterThanOrEqualTo(2);
        }

        [Fact]
        public async Task GetAllByUserIdAsync_ShouldReturnOnlyUserDiaries()
        {
            var userId = 20;
            await _service.CreateAsync(new CreateDiaryDTO(userId, DateOnly.FromDateTime(DateTime.UtcNow)));
            await _service.CreateAsync(new CreateDiaryDTO(21, DateOnly.FromDateTime(DateTime.UtcNow)));

            var result = await _service.GetAllByUserIdAsync(userId, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateDiary()
        {
            var dto = new CreateDiaryDTO(3, DateOnly.FromDateTime(DateTime.UtcNow));
            var createResult = await _service.CreateAsync(dto);
            var newDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            // Use a fresh context so the previously-tracked entity does not conflict
            RecreateService();

            var result = await _service.UpdateAsync(new UpdateDiaryDTO(createResult.Value, newDate));

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteDiary()
        {
            var dto = new CreateDiaryDTO(4, DateOnly.FromDateTime(DateTime.UtcNow));
            var createResult = await _service.CreateAsync(dto);

            // Use a fresh context so the previously-tracked entity does not conflict
            RecreateService();

            var result = await _service.DeleteAsync(createResult.Value);

            result.IsSuccess.Should().BeTrue();

            var getResult = await _service.GetByIdAsync(createResult.Value);
            getResult.IsSuccess.Should().BeFalse();
        }
    }

    /// <summary>
    /// Minimal fake publisher for integration tests that do not need actual event publishing.
    /// </summary>
    internal class FakePublisher : BuildingBlocks.CQRS.Publisher.IPublisher
    {
        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : BuildingBlocks.CQRS.Notification.INotification
        {
            return Task.CompletedTask;
        }
    }
}
