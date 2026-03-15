using Gym.Domain.Entities;
using Gym.Infrastructure.Persistence.Repositories;
using Gym.IntegrationTests.Fixtures;
using Gym.IntegrationTests.Helpers;
using FluentAssertions;

namespace Gym.IntegrationTests.Repositories
{
    [Trait("Category", "Integration")]
    [Trait("Layer", "Infrastructure")]
    public class TrainingSessionRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private TrainingSessionRepository _repository;

        public TrainingSessionRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new TrainingSessionRepository(_fixture.DbContext);
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            _repository = new TrainingSessionRepository(_fixture.CreateNewContext());
        }

        public Task DisposeAsync() => Task.CompletedTask;

        #region Helpers

        private async Task<Routine> CreateRoutineAsync()
        {
            var context = _fixture.CreateNewContext();
            var routine = TestDataFactory.CreateRoutine();
            await context.Routines.AddAsync(routine);
            await context.SaveChangesAsync();
            return routine;
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_WithValidEntity_ShouldPersist()
        {
            // Arrange
            var routine = await CreateRoutineAsync();
            var session = TestDataFactory.CreateTrainingSession(routineId: routine.Id);
            var repo = new TrainingSessionRepository(_fixture.CreateNewContext());

            // Act
            await repo.Create(session);

            // Assert
            var verifyRepo = new TrainingSessionRepository(_fixture.CreateNewContext());
            var saved = await verifyRepo.GetById(session.Id);
            saved.Should().NotBeNull();
            saved!.RoutineId.Should().Be(routine.Id);
            saved.UserId.Should().Be(1);
        }

        [Fact]
        public async Task CreateRange_WithMultipleEntities_ShouldPersistAll()
        {
            // Arrange
            var routine = await CreateRoutineAsync();
            var sessions = new List<TrainingSession>
            {
                TestDataFactory.CreateTrainingSession(routineId: routine.Id),
                TestDataFactory.CreateTrainingSession(userId: 2, routineId: routine.Id),
                TestDataFactory.CreateTrainingSession(userId: 3, routineId: routine.Id)
            };
            var repo = new TrainingSessionRepository(_fixture.CreateNewContext());

            // Act
            await repo.CreateRange(sessions);

            // Assert
            var verifyRepo = new TrainingSessionRepository(_fixture.CreateNewContext());
            var all = await verifyRepo.GetAll();
            all.Should().HaveCount(3);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_WithExistingId_ShouldReturnEntityWithRoutine()
        {
            // Arrange
            var routine = await CreateRoutineAsync();
            var session = TestDataFactory.CreateTrainingSession(routineId: routine.Id);
            var repoCreate = new TrainingSessionRepository(_fixture.CreateNewContext());
            await repoCreate.Create(session);

            // Act
            var repo = new TrainingSessionRepository(_fixture.CreateNewContext());
            var result = await repo.GetById(session.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(session.Id);
            result.Routine.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_WithNonExistingId_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetById(99999);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ShouldReturnAllSessions()
        {
            // Arrange
            var routine = await CreateRoutineAsync();
            var session = TestDataFactory.CreateTrainingSession(routineId: routine.Id);
            var repoCreate = new TrainingSessionRepository(_fixture.CreateNewContext());
            await repoCreate.Create(session);

            // Act
            var repo = new TrainingSessionRepository(_fixture.CreateNewContext());
            var result = await repo.GetAll();

            // Assert
            result.Should().NotBeEmpty();
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_ShouldPersistChanges()
        {
            // Arrange
            var routine = await CreateRoutineAsync();
            var session = TestDataFactory.CreateTrainingSession(routineId: routine.Id);
            var repoCreate = new TrainingSessionRepository(_fixture.CreateNewContext());
            await repoCreate.Create(session);

            // Act
            var repoUpdate = new TrainingSessionRepository(_fixture.CreateNewContext());
            var toUpdate = await repoUpdate.GetById(session.Id);
            toUpdate!.Complete("Session completed with notes");
            await repoUpdate.Update(toUpdate);

            // Assert
            var verifyRepo = new TrainingSessionRepository(_fixture.CreateNewContext());
            var updated = await verifyRepo.GetById(session.Id);
            updated!.Notes.Should().Be("Session completed with notes");
            updated.EndTime.Should().NotBeNull();
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            var routine = await CreateRoutineAsync();
            var session = TestDataFactory.CreateTrainingSession(routineId: routine.Id);
            var repoCreate = new TrainingSessionRepository(_fixture.CreateNewContext());
            await repoCreate.Create(session);

            // Act
            var repoDelete = new TrainingSessionRepository(_fixture.CreateNewContext());
            var toDelete = await repoDelete.GetById(session.Id);
            await repoDelete.Delete(toDelete!);

            // Assert
            var verifyRepo = new TrainingSessionRepository(_fixture.CreateNewContext());
            var deleted = await verifyRepo.GetById(session.Id);
            deleted.Should().BeNull();
        }

        #endregion

        #region Find Tests

        [Fact]
        public async Task Find_ByUserId_ShouldReturnMatchingSessions()
        {
            // Arrange
            var routine = await CreateRoutineAsync();
            var session = TestDataFactory.CreateTrainingSession(userId: 42, routineId: routine.Id);
            var repoCreate = new TrainingSessionRepository(_fixture.CreateNewContext());
            await repoCreate.Create(session);

            // Act
            var repo = new TrainingSessionRepository(_fixture.CreateNewContext());
            var result = await repo.Find(ts => ts.UserId == 42);

            // Assert
            result.Should().NotBeEmpty();
            result.Should().AllSatisfy(ts => ts!.UserId.Should().Be(42));
        }

        #endregion
    }
}
