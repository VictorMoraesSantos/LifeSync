using FluentAssertions;
using Gym.Domain.Entities;
using Gym.Domain.Enums;
using Gym.UnitTests.Helpers.Builders;

namespace Gym.UnitTests.Domain.Entities;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class ExerciseTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateEntity()
    {
        // Arrange
        var name = "Bench Press";
        var description = "Flat bench barbell press";
        var muscleGroup = MuscleGroup.Chest;
        var type = ExerciseType.Strength;
        var equipmentType = EquipmentType.Barbell;

        // Act
        var exercise = new Exercise(name, description, muscleGroup, type, equipmentType);

        // Assert
        exercise.Should().NotBeNull();
        exercise.Name.Should().Be(name);
        exercise.Description.Should().Be(description);
        exercise.MuscleGroup.Should().Be(muscleGroup);
        exercise.Type.Should().Be(type);
        exercise.EquipmentType.Should().Be(equipmentType);
    }

    [Fact]
    public void Create_WithNullEquipmentType_ShouldCreateEntityWithNullEquipment()
    {
        // Arrange & Act
        var exercise = ExerciseBuilder.Default()
            .WithEquipmentType(null)
            .Build();

        // Assert
        exercise.Should().NotBeNull();
        exercise.EquipmentType.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_WithInvalidName_ShouldThrowArgumentException(string? invalidName)
    {
        // Arrange & Act
        var action = () => new Exercise(invalidName!, "Valid description", MuscleGroup.Chest, ExerciseType.Strength);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_WithInvalidDescription_ShouldThrowArgumentException(string? invalidDescription)
    {
        // Arrange & Act
        var action = () => new Exercise("Valid Name", invalidDescription!, MuscleGroup.Chest, ExerciseType.Strength);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Update_WithValidParameters_ShouldUpdateProperties()
    {
        // Arrange
        var exercise = ExerciseBuilder.Default().Build();
        var newName = "Incline Press";
        var newDescription = "Incline bench barbell press";
        var newMuscleGroup = MuscleGroup.Shoulders;
        var newType = ExerciseType.Hypertrophy;
        var newEquipment = EquipmentType.Dumbbell;

        // Act
        exercise.Update(newName, newDescription, newMuscleGroup, newType, newEquipment);

        // Assert
        exercise.Name.Should().Be(newName);
        exercise.Description.Should().Be(newDescription);
        exercise.MuscleGroup.Should().Be(newMuscleGroup);
        exercise.Type.Should().Be(newType);
        exercise.EquipmentType.Should().Be(newEquipment);
        exercise.UpdatedAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Update_WithInvalidName_ShouldThrowArgumentException(string? invalidName)
    {
        // Arrange
        var exercise = ExerciseBuilder.Default().Build();

        // Act
        var action = () => exercise.Update(invalidName!, "Valid description", MuscleGroup.Chest, ExerciseType.Strength, null);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Update_WithInvalidDescription_ShouldThrowArgumentException(string? invalidDescription)
    {
        // Arrange
        var exercise = ExerciseBuilder.Default().Build();

        // Act
        var action = () => exercise.Update("Valid Name", invalidDescription!, MuscleGroup.Chest, ExerciseType.Strength, null);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SetEquipmentType_WithValidType_ShouldUpdateEquipment()
    {
        // Arrange
        var exercise = ExerciseBuilder.Default().WithEquipmentType(null).Build();

        // Act
        exercise.SetEquipmentType(EquipmentType.Cable);

        // Assert
        exercise.EquipmentType.Should().Be(EquipmentType.Cable);
        exercise.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void SetEquipmentType_WithNull_ShouldClearEquipment()
    {
        // Arrange
        var exercise = ExerciseBuilder.Default().WithEquipmentType(EquipmentType.Barbell).Build();

        // Act
        exercise.SetEquipmentType(null);

        // Assert
        exercise.EquipmentType.Should().BeNull();
    }

    [Theory]
    [InlineData(MuscleGroup.Chest)]
    [InlineData(MuscleGroup.Back)]
    [InlineData(MuscleGroup.Shoulders)]
    [InlineData(MuscleGroup.Biceps)]
    [InlineData(MuscleGroup.Triceps)]
    [InlineData(MuscleGroup.Quadriceps)]
    [InlineData(MuscleGroup.Hamstrings)]
    [InlineData(MuscleGroup.Calves)]
    [InlineData(MuscleGroup.Glutes)]
    [InlineData(MuscleGroup.Abs)]
    [InlineData(MuscleGroup.FullBody)]
    [InlineData(MuscleGroup.Core)]
    [InlineData(MuscleGroup.LowerBack)]
    [InlineData(MuscleGroup.Traps)]
    [InlineData(MuscleGroup.Neck)]
    [InlineData(MuscleGroup.Forearms)]
    public void Create_WithAllMuscleGroups_ShouldCreateSuccessfully(MuscleGroup muscleGroup)
    {
        // Arrange & Act
        var exercise = ExerciseBuilder.Default()
            .WithMuscleGroup(muscleGroup)
            .Build();

        // Assert
        exercise.MuscleGroup.Should().Be(muscleGroup);
    }

    [Theory]
    [InlineData(ExerciseType.Strength)]
    [InlineData(ExerciseType.Hypertrophy)]
    [InlineData(ExerciseType.Endurance)]
    [InlineData(ExerciseType.Power)]
    [InlineData(ExerciseType.Flexibility)]
    [InlineData(ExerciseType.Cardio)]
    [InlineData(ExerciseType.HIIT)]
    [InlineData(ExerciseType.Recovery)]
    public void Create_WithAllExerciseTypes_ShouldCreateSuccessfully(ExerciseType exerciseType)
    {
        // Arrange & Act
        var exercise = ExerciseBuilder.Default()
            .WithType(exerciseType)
            .Build();

        // Assert
        exercise.Type.Should().Be(exerciseType);
    }

    [Fact]
    public void Update_ShouldMarkAsUpdated()
    {
        // Arrange
        var exercise = ExerciseBuilder.Default().Build();
        var originalCreatedAt = exercise.CreatedAt;
        Thread.Sleep(10);

        // Act
        exercise.Update("New Name", "New Desc", MuscleGroup.Back, ExerciseType.Cardio, EquipmentType.Cable);

        // Assert
        exercise.UpdatedAt.Should().NotBeNull();
        exercise.UpdatedAt.Should().BeAfter(originalCreatedAt);
    }
}
