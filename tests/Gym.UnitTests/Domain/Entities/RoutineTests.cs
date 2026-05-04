using Core.Domain.Exceptions;
using FluentAssertions;
using Gym.Domain.Entities;
using Gym.Domain.ValueObjects;
using Gym.UnitTests.Helpers.Builders;

namespace Gym.UnitTests.Domain.Entities;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class RoutineTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateEntity()
    {
        // Arrange
        var name = "Push Day";
        var description = "Chest, shoulders and triceps";

        // Act
        var routine = new Routine(name, description);

        // Assert
        routine.Should().NotBeNull();
        routine.Name.Should().Be(name);
        routine.Description.Should().Be(description);
        routine.RoutineExercises.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_WithInvalidName_ShouldThrowArgumentException(string? invalidName)
    {
        // Arrange & Act
        var action = () => new Routine(invalidName!, "Valid description");

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
        var action = () => new Routine("Valid Name", invalidDescription!);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Update_WithValidParameters_ShouldUpdateProperties()
    {
        // Arrange
        var routine = RoutineBuilder.Default().Build();
        var newName = "Pull Day";
        var newDescription = "Back and biceps workout";

        // Act
        routine.Update(newName, newDescription);

        // Assert
        routine.Name.Should().Be(newName);
        routine.Description.Should().Be(newDescription);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Update_WithInvalidName_ShouldThrowArgumentException(string? invalidName)
    {
        // Arrange
        var routine = RoutineBuilder.Default().Build();

        // Act
        var action = () => routine.Update(invalidName!, "Valid description");

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
        var routine = RoutineBuilder.Default().Build();

        // Act
        var action = () => routine.Update("Valid Name", invalidDescription!);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddExercise_WithValidRoutineExercise_ShouldAddToCollection()
    {
        // Arrange
        var routine = RoutineBuilder.Default().Build();
        var routineExercise = new RoutineExercise(
            routineId: 1,
            exerciseId: 1,
            sets: SetCount.Create(3),
            repetitions: RepetitionCount.Create(10),
            restBetweenSets: RestTime.Create(60));

        // Act
        routine.AddExercise(routineExercise);

        // Assert
        routine.RoutineExercises.Should().ContainSingle();
        routine.RoutineExercises.Should().Contain(routineExercise);
        routine.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void AddExercise_WithNull_ShouldThrowDomainException()
    {
        // Arrange
        var routine = RoutineBuilder.Default().Build();

        // Act
        var action = () => routine.AddExercise(null!);

        // Assert
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void RemoveExercise_WithExistingRoutineExercise_ShouldRemoveFromCollection()
    {
        // Arrange
        var routine = RoutineBuilder.Default().Build();
        var routineExercise = new RoutineExercise(
            routineId: 1,
            exerciseId: 1,
            sets: SetCount.Create(3),
            repetitions: RepetitionCount.Create(10),
            restBetweenSets: RestTime.Create(60));
        routine.AddExercise(routineExercise);

        // Act
        routine.RemoveExercise(routineExercise);

        // Assert
        routine.RoutineExercises.Should().BeEmpty();
    }

    [Fact]
    public void RemoveExercise_WithNull_ShouldThrowDomainException()
    {
        // Arrange
        var routine = RoutineBuilder.Default().Build();

        // Act
        var action = () => routine.RemoveExercise(null!);

        // Assert
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void RoutineExercises_ShouldBeReadOnly()
    {
        // Arrange
        var routine = RoutineBuilder.Default().Build();

        // Assert
        routine.RoutineExercises.Should().BeAssignableTo<IReadOnlyCollection<RoutineExercise>>();
    }

    [Fact]
    public void AddExercise_MultipleExercises_ShouldAddAll()
    {
        // Arrange
        var routine = RoutineBuilder.Default().Build();
        var exercise1 = new RoutineExercise(1, 1, SetCount.Create(3), RepetitionCount.Create(10), RestTime.Create(60));
        var exercise2 = new RoutineExercise(1, 2, SetCount.Create(4), RepetitionCount.Create(8), RestTime.Create(90));

        // Act
        routine.AddExercise(exercise1);
        routine.AddExercise(exercise2);

        // Assert
        routine.RoutineExercises.Should().HaveCount(2);
    }
}
