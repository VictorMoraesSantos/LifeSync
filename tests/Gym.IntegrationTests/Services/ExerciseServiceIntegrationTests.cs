using FluentAssertions;
using Gym.Application.DTOs.Exercise;
using Gym.Domain.Enums;
using Gym.Infrastructure.Persistence.Repositories;
using Gym.Infrastructure.Services;
using Gym.IntegrationTests.Fixtures;
using Microsoft.Extensions.Logging.Abstractions;

namespace Gym.IntegrationTests.Services
{
    [Trait("Category", "Integration")]
    [Trait("Layer", "Infrastructure")]
    public class ExerciseServiceIntegrationTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;

        public ExerciseServiceIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private ExerciseService CreateService()
        {
            var context = _fixture.CreateNewContext();
            var repo = new ExerciseRepository(context);
            return new ExerciseService(repo, NullLogger<ExerciseService>.Instance);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldCreateExercise()
        {
            // Arrange
            var service = CreateService();
            var dto = new CreateExerciseDTO("Bench Press", "Flat bench press", MuscleGroup.Chest, ExerciseType.Strength, EquipmentType.Barbell);

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CreateAsync_WithNullDto_ShouldReturnFailure()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.CreateAsync(null!);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithExistingExercise_ShouldReturnDTO()
        {
            // Arrange
            var service = CreateService();
            var createResult = await service.CreateAsync(
                new CreateExerciseDTO("Squat", "Barbell squat", MuscleGroup.Quadriceps, ExerciseType.Strength, EquipmentType.Barbell));

            // Act
            var service2 = CreateService();
            var result = await service2.GetByIdAsync(createResult.Value);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Name.Should().Be("Squat");
            result.Value.MuscleGroup.Should().Be(MuscleGroup.Quadriceps);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ShouldReturnFailure()
        {
            // Act
            var service = CreateService();
            var result = await service.GetByIdAsync(99999);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_WithExistingExercises_ShouldReturnAll()
        {
            // Arrange
            var service = CreateService();
            await service.CreateAsync(new CreateExerciseDTO("Exercise 1", "Desc 1", MuscleGroup.Chest, ExerciseType.Strength, EquipmentType.Barbell));
            await service.CreateAsync(new CreateExerciseDTO("Exercise 2", "Desc 2", MuscleGroup.Back, ExerciseType.Hypertrophy, EquipmentType.Dumbbell));

            // Act
            var service2 = CreateService();
            var result = await service2.GetAllAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateExercise()
        {
            // Arrange
            var service = CreateService();
            var createResult = await service.CreateAsync(
                new CreateExerciseDTO("Old Name", "Old Desc", MuscleGroup.Chest, ExerciseType.Strength, EquipmentType.Barbell));

            var updateDto = new UpdateExerciseDTO(createResult.Value, "New Name", "New Desc", MuscleGroup.Back, ExerciseType.Hypertrophy, EquipmentType.Dumbbell);

            // Act
            var service2 = CreateService();
            var result = await service2.UpdateAsync(updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var service3 = CreateService();
            var updated = await service3.GetByIdAsync(createResult.Value);
            updated.Value!.Name.Should().Be("New Name");
            updated.Value.MuscleGroup.Should().Be(MuscleGroup.Back);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WithExistingExercise_ShouldSucceed()
        {
            // Arrange
            var service = CreateService();
            var createResult = await service.CreateAsync(
                new CreateExerciseDTO("To Delete", "Will be deleted", MuscleGroup.Chest, ExerciseType.Strength, EquipmentType.Barbell));

            // Act
            var service2 = CreateService();
            var result = await service2.DeleteAsync(createResult.Value);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var service3 = CreateService();
            var getResult = await service3.GetByIdAsync(createResult.Value);
            getResult.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingId_ShouldReturnFailure()
        {
            // Act
            var service = CreateService();
            var result = await service.DeleteAsync(99999);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion
    }
}
