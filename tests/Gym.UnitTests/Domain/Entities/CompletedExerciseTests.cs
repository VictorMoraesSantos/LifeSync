using FluentAssertions;
using Gym.Domain.Entities;
using Gym.Domain.Enums;
using Gym.Domain.ValueObjects;
using Gym.UnitTests.Helpers.Builders;

namespace Gym.UnitTests.Domain.Entities;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class CompletedExerciseTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateEntity()
    {
        // Arrange
        var trainingSessionId = 1;
        var routineExerciseId = 1;
        var sets = SetCount.Create(3);
        var reps = RepetitionCount.Create(10);
        var weight = Weight.Create(80, MeasurementUnit.Kilogram);
        var notes = "Good form";

        // Act
        var completedExercise = new CompletedExercise(trainingSessionId, routineExerciseId, sets, reps, weight, notes);

        // Assert
        completedExercise.Should().NotBeNull();
        completedExercise.TrainingSessionId.Should().Be(trainingSessionId);
        completedExercise.RoutineExerciseId.Should().Be(routineExerciseId);
        completedExercise.SetsCompleted.Should().Be(sets);
        completedExercise.RepetitionsCompleted.Should().Be(reps);
        completedExercise.WeightUsed.Should().Be(weight);
        completedExercise.Notes.Should().Be(notes);
    }

    [Fact]
    public void Create_WithNullWeight_ShouldCreateEntity()
    {
        // Arrange & Act
        var completedExercise = CompletedExerciseBuilder.Default()
            .WithWeightUsed(null)
            .Build();

        // Assert
        completedExercise.WeightUsed.Should().BeNull();
    }

    [Fact]
    public void Create_WithNullNotes_ShouldCreateEntity()
    {
        // Arrange & Act
        var completedExercise = CompletedExerciseBuilder.Default()
            .WithNotes(null)
            .Build();

        // Assert
        completedExercise.Notes.Should().BeNull();
    }

    [Fact]
    public void Update_WithValidParameters_ShouldUpdateProperties()
    {
        // Arrange
        var completedExercise = CompletedExerciseBuilder.Default().Build();
        var newSets = SetCount.Create(4);
        var newReps = RepetitionCount.Create(12);
        var newWeight = Weight.Create(100, MeasurementUnit.Kilogram);
        var newNotes = "Increased weight";

        // Act
        completedExercise.Update(newSets, newReps, newWeight, newNotes);

        // Assert
        completedExercise.SetsCompleted.Should().Be(newSets);
        completedExercise.RepetitionsCompleted.Should().Be(newReps);
        completedExercise.WeightUsed.Should().Be(newWeight);
        completedExercise.Notes.Should().Be(newNotes);
        completedExercise.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Update_WithNullWeight_ShouldClearWeight()
    {
        // Arrange
        var completedExercise = CompletedExerciseBuilder.Default().Build();

        // Act
        completedExercise.Update(SetCount.Create(3), RepetitionCount.Create(10), null, null);

        // Assert
        completedExercise.WeightUsed.Should().BeNull();
    }

    [Fact]
    public void MarkCompleted_ShouldSetCompletedAtAndMarkAsUpdated()
    {
        // Arrange
        var completedExercise = CompletedExerciseBuilder.Default().Build();
        var beforeMark = DateTime.UtcNow;

        // Act
        completedExercise.MarkCompleted();

        // Assert
        completedExercise.CompletedAt.Should().BeOnOrAfter(beforeMark);
        completedExercise.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkCompleted_CalledTwice_ShouldUpdateCompletedAt()
    {
        // Arrange
        var completedExercise = CompletedExerciseBuilder.Default().Build();
        completedExercise.MarkCompleted();
        var firstCompletedAt = completedExercise.CompletedAt;
        Thread.Sleep(10);

        // Act
        completedExercise.MarkCompleted();

        // Assert
        completedExercise.CompletedAt.Should().BeOnOrAfter(firstCompletedAt);
    }

    [Fact]
    public void Create_WithBuilderDefaults_ShouldHaveExpectedValues()
    {
        // Arrange & Act
        var completedExercise = CompletedExerciseBuilder.Default().Build();

        // Assert
        completedExercise.TrainingSessionId.Should().Be(1);
        completedExercise.RoutineExerciseId.Should().Be(1);
        completedExercise.SetsCompleted.Value.Should().Be(3);
        completedExercise.RepetitionsCompleted.Value.Should().Be(10);
        completedExercise.WeightUsed.Should().NotBeNull();
        completedExercise.WeightUsed!.Value.Should().Be(60);
    }
}
