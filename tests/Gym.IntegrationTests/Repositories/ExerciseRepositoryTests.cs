using FluentAssertions;
using Gym.Domain.Enums;
using Gym.Infrastructure.Persistence.Repositories;
using Gym.IntegrationTests.Fixtures;
using Gym.IntegrationTests.Helpers;

namespace Gym.IntegrationTests.Repositories
{
    [Trait("Category", "Integration")]
    [Trait("Layer", "Infrastructure")]
    public class ExerciseRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private ExerciseRepository _repository;

        public ExerciseRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new ExerciseRepository(_fixture.DbContext);
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            _repository = new ExerciseRepository(_fixture.CreateNewContext());
        }

        public Task DisposeAsync() => Task.CompletedTask;

        #region Create Tests

        [Fact]
        public async Task Create_WithValidEntity_ShouldPersist()
        {
            // Arrange
            var exercise = TestDataFactory.CreateExercise();
            var repo = new ExerciseRepository(_fixture.CreateNewContext());

            // Act
            await repo.Create(exercise);

            // Assert
            var verifyRepo = new ExerciseRepository(_fixture.CreateNewContext());
            var saved = await verifyRepo.GetById(exercise.Id);
            saved.Should().NotBeNull();
            saved!.Name.Should().Be(exercise.Name);
            saved.MuscleGroup.Should().Be(exercise.MuscleGroup);
        }

        [Fact]
        public async Task CreateRange_WithMultipleEntities_ShouldPersistAll()
        {
            // Arrange
            var exercises = TestDataFactory.CreateExercises(3);
            var repo = new ExerciseRepository(_fixture.CreateNewContext());

            // Act
            await repo.CreateRange(exercises);

            // Assert
            var verifyRepo = new ExerciseRepository(_fixture.CreateNewContext());
            var all = await verifyRepo.GetAll();
            all.Should().HaveCount(3);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_WithExistingId_ShouldReturnEntity()
        {
            // Arrange
            var exercise = TestDataFactory.CreateExercise();
            var repoCreate = new ExerciseRepository(_fixture.CreateNewContext());
            await repoCreate.Create(exercise);

            // Act
            var repo = new ExerciseRepository(_fixture.CreateNewContext());
            var result = await repo.GetById(exercise.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(exercise.Id);
            result.Name.Should().Be(exercise.Name);
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
        public async Task GetAll_ShouldReturnAllExercises()
        {
            // Arrange
            var exercises = TestDataFactory.CreateExercises(2);
            var repoCreate = new ExerciseRepository(_fixture.CreateNewContext());
            await repoCreate.CreateRange(exercises);

            // Act
            var repo = new ExerciseRepository(_fixture.CreateNewContext());
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
            var exercise = TestDataFactory.CreateExercise();
            var repoCreate = new ExerciseRepository(_fixture.CreateNewContext());
            await repoCreate.Create(exercise);

            // Act
            var repoUpdate = new ExerciseRepository(_fixture.CreateNewContext());
            var toUpdate = await repoUpdate.GetById(exercise.Id);
            toUpdate!.Update("Updated Name", "Updated Description", MuscleGroup.Back, ExerciseType.Strength, EquipmentType.Barbell);
            await repoUpdate.Update(toUpdate);

            // Assert
            var verifyRepo = new ExerciseRepository(_fixture.CreateNewContext());
            var updated = await verifyRepo.GetById(exercise.Id);
            updated!.Name.Should().Be("Updated Name");
            updated.Description.Should().Be("Updated Description");
            updated.MuscleGroup.Should().Be(MuscleGroup.Back);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            var exercise = TestDataFactory.CreateExercise();
            var repoCreate = new ExerciseRepository(_fixture.CreateNewContext());
            await repoCreate.Create(exercise);

            // Act
            var repoDelete = new ExerciseRepository(_fixture.CreateNewContext());
            var toDelete = await repoDelete.GetById(exercise.Id);
            await repoDelete.Delete(toDelete!);

            // Assert
            var verifyRepo = new ExerciseRepository(_fixture.CreateNewContext());
            var deleted = await verifyRepo.GetById(exercise.Id);
            deleted.Should().BeNull();
        }

        #endregion

        #region Find Tests

        [Fact]
        public async Task Find_WithPredicate_ShouldReturnMatchingEntities()
        {
            // Arrange
            var exercise = TestDataFactory.CreateExercise(muscleGroup: MuscleGroup.Chest);
            var repoCreate = new ExerciseRepository(_fixture.CreateNewContext());
            await repoCreate.Create(exercise);

            // Act
            var repo = new ExerciseRepository(_fixture.CreateNewContext());
            var result = await repo.Find(e => e.MuscleGroup == MuscleGroup.Chest);

            // Assert
            result.Should().NotBeEmpty();
            result.Should().AllSatisfy(e => e!.MuscleGroup.Should().Be(MuscleGroup.Chest));
        }

        #endregion
    }
}
