using FluentAssertions;
using Gym.Infrastructure.Persistence.Repositories;
using Gym.IntegrationTests.Fixtures;
using Gym.IntegrationTests.Helpers;

namespace Gym.IntegrationTests.Repositories
{
    [Trait("Category", "Integration")]
    [Trait("Layer", "Infrastructure")]
    public class RoutineRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private RoutineRepository _repository;

        public RoutineRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new RoutineRepository(_fixture.DbContext);
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            _repository = new RoutineRepository(_fixture.CreateNewContext());
        }

        public Task DisposeAsync() => Task.CompletedTask;

        #region Create Tests

        [Fact]
        public async Task Create_WithValidEntity_ShouldPersist()
        {
            // Arrange
            var routine = TestDataFactory.CreateRoutine();
            var repo = new RoutineRepository(_fixture.CreateNewContext());

            // Act
            await repo.Create(routine);

            // Assert
            var verifyRepo = new RoutineRepository(_fixture.CreateNewContext());
            var saved = await verifyRepo.GetById(routine.Id);
            saved.Should().NotBeNull();
            saved!.Name.Should().Be(routine.Name);
        }

        [Fact]
        public async Task CreateRange_WithMultipleEntities_ShouldPersistAll()
        {
            // Arrange
            var routines = TestDataFactory.CreateRoutines(3);
            var repo = new RoutineRepository(_fixture.CreateNewContext());

            // Act
            await repo.CreateRange(routines);

            // Assert
            var verifyRepo = new RoutineRepository(_fixture.CreateNewContext());
            var all = await verifyRepo.GetAll();
            all.Should().HaveCount(3);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_WithExistingId_ShouldReturnEntity()
        {
            // Arrange
            var routine = TestDataFactory.CreateRoutine();
            var repoCreate = new RoutineRepository(_fixture.CreateNewContext());
            await repoCreate.Create(routine);

            // Act
            var repo = new RoutineRepository(_fixture.CreateNewContext());
            var result = await repo.GetById(routine.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(routine.Id);
            result.Name.Should().Be(routine.Name);
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
        public async Task GetAll_ShouldReturnAllRoutines()
        {
            // Arrange
            var routines = TestDataFactory.CreateRoutines(2);
            var repoCreate = new RoutineRepository(_fixture.CreateNewContext());
            await repoCreate.CreateRange(routines);

            // Act
            var repo = new RoutineRepository(_fixture.CreateNewContext());
            var result = await repo.GetAll();

            // Assert
            result.Should().HaveCount(2);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_ShouldPersistChanges()
        {
            // Arrange
            var routine = TestDataFactory.CreateRoutine();
            var repoCreate = new RoutineRepository(_fixture.CreateNewContext());
            await repoCreate.Create(routine);

            // Act
            var repoUpdate = new RoutineRepository(_fixture.CreateNewContext());
            var toUpdate = await repoUpdate.GetById(routine.Id);
            toUpdate!.Update("Updated Routine", "Updated Description");
            await repoUpdate.Update(toUpdate);

            // Assert
            var verifyRepo = new RoutineRepository(_fixture.CreateNewContext());
            var updated = await verifyRepo.GetById(routine.Id);
            updated!.Name.Should().Be("Updated Routine");
            updated.Description.Should().Be("Updated Description");
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            var routine = TestDataFactory.CreateRoutine();
            var repoCreate = new RoutineRepository(_fixture.CreateNewContext());
            await repoCreate.Create(routine);

            // Act
            var repoDelete = new RoutineRepository(_fixture.CreateNewContext());
            var toDelete = await repoDelete.GetById(routine.Id);
            await repoDelete.Delete(toDelete!);

            // Assert
            var verifyRepo = new RoutineRepository(_fixture.CreateNewContext());
            var deleted = await verifyRepo.GetById(routine.Id);
            deleted.Should().BeNull();
        }

        #endregion

        #region Find Tests

        [Fact]
        public async Task Find_WithPredicate_ShouldReturnMatchingEntities()
        {
            // Arrange
            var routine = TestDataFactory.CreateRoutine("Special Routine");
            var repoCreate = new RoutineRepository(_fixture.CreateNewContext());
            await repoCreate.Create(routine);

            // Act
            var repo = new RoutineRepository(_fixture.CreateNewContext());
            var result = await repo.Find(r => r.Name == "Special Routine");

            // Assert
            result.Should().NotBeEmpty();
            result.Should().AllSatisfy(r => r!.Name.Should().Be("Special Routine"));
        }

        #endregion
    }
}
