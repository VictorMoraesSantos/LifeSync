using Core.Domain.Exceptions;
using FluentAssertions;
using Gym.Domain.Entities;
using Gym.UnitTests.Helpers.Builders;

namespace Gym.UnitTests.Domain.Entities;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class TrainingSessionTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateEntity()
    {
        // Arrange
        var userId = 1;
        var routineId = 1;
        var startTime = DateTime.UtcNow.AddHours(-1);

        // Act
        var session = new TrainingSession(userId, routineId, startTime, null);

        // Assert
        session.Should().NotBeNull();
        session.UserId.Should().Be(userId);
        session.RoutineId.Should().Be(routineId);
        session.StartTime.Should().Be(startTime);
        session.EndTime.Should().BeNull();
        session.Notes.Should().BeNull();
        session.CompletedExercises.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithValidEndTime_ShouldCreateEntity()
    {
        // Arrange
        var startTime = DateTime.UtcNow.AddHours(-1);
        var endTime = DateTime.UtcNow;

        // Act
        var session = new TrainingSession(1, 1, startTime, endTime);

        // Assert
        session.EndTime.Should().Be(endTime);
    }

    [Fact]
    public void Create_WithEndTimeBeforeStartTime_ShouldThrowDomainException()
    {
        // Arrange
        var startTime = DateTime.UtcNow;
        var endTime = DateTime.UtcNow.AddHours(-1);

        // Act
        var action = () => new TrainingSession(1, 1, startTime, endTime);

        // Assert
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_WithEndTimeEqualToStartTime_ShouldThrowDomainException()
    {
        // Arrange
        var time = DateTime.UtcNow;

        // Act
        var action = () => new TrainingSession(1, 1, time, time);

        // Assert
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Update_WithValidParameters_ShouldUpdateProperties()
    {
        // Arrange
        var session = TrainingSessionBuilder.Default().Build();
        var newRoutineId = 2;
        var newStartTime = DateTime.UtcNow.AddHours(-2);
        var newEndTime = DateTime.UtcNow.AddMinutes(-30);
        var newNotes = "Great workout";

        // Act
        session.Update(newRoutineId, newStartTime, newEndTime, newNotes);

        // Assert
        session.RoutineId.Should().Be(newRoutineId);
        session.StartTime.Should().Be(newStartTime);
        session.EndTime.Should().Be(newEndTime);
        session.Notes.Should().Be(newNotes);
        session.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Update_WithEndTimeBeforeStartTime_ShouldThrowDomainException()
    {
        // Arrange
        var session = TrainingSessionBuilder.Default().Build();
        var startTime = DateTime.UtcNow;
        var endTime = DateTime.UtcNow.AddHours(-1);

        // Act
        var action = () => session.Update(1, startTime, endTime, "notes");

        // Assert
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddCompletedExercise_WithValidExercise_ShouldAddToCollection()
    {
        // Arrange
        var session = TrainingSessionBuilder.Default().Build();
        var completedExercise = CompletedExerciseBuilder.Default().Build();

        // Act
        session.AddCompletedExercise(completedExercise);

        // Assert
        session.CompletedExercises.Should().ContainSingle();
        session.CompletedExercises.Should().Contain(completedExercise);
        session.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void AddCompletedExercise_WithNull_ShouldThrowDomainException()
    {
        // Arrange
        var session = TrainingSessionBuilder.Default().Build();

        // Act
        var action = () => session.AddCompletedExercise(null!);

        // Assert
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Complete_WithoutNotes_ShouldSetEndTime()
    {
        // Arrange
        var session = TrainingSessionBuilder.Default().Build();

        // Act
        session.Complete();

        // Assert
        session.EndTime.Should().NotBeNull();
        session.Notes.Should().BeNull();
        session.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Complete_WithNotes_ShouldSetEndTimeAndNotes()
    {
        // Arrange
        var session = TrainingSessionBuilder.Default().Build();
        var notes = "Felt strong today";

        // Act
        session.Complete(notes);

        // Assert
        session.EndTime.Should().NotBeNull();
        session.Notes.Should().Be(notes);
    }

    [Fact]
    public void Complete_WhenAlreadyCompleted_ShouldNotUpdateEndTime()
    {
        // Arrange
        var session = TrainingSessionBuilder.Default().Build();
        session.Complete("First completion");
        var firstEndTime = session.EndTime;
        Thread.Sleep(10);

        // Act
        session.Complete("Second attempt");

        // Assert
        session.EndTime.Should().Be(firstEndTime);
        session.Notes.Should().Be("First completion");
    }

    [Fact]
    public void GetDuration_WhenCompleted_ShouldReturnActualDuration()
    {
        // Arrange
        var startTime = DateTime.UtcNow.AddHours(-1);
        var endTime = DateTime.UtcNow;
        var session = new TrainingSession(1, 1, startTime, endTime);

        // Act
        var duration = session.GetDuration();

        // Assert
        duration.Should().BeCloseTo(TimeSpan.FromHours(1), TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void GetDuration_WhenNotCompleted_ShouldReturnDurationFromStart()
    {
        // Arrange
        var startTime = DateTime.UtcNow.AddMinutes(-30);
        var session = new TrainingSession(1, 1, startTime, null);

        // Act
        var duration = session.GetDuration();

        // Assert
        duration.Should().BeCloseTo(TimeSpan.FromMinutes(30), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void CompletedExercises_ShouldBeReadOnly()
    {
        // Arrange
        var session = TrainingSessionBuilder.Default().Build();

        // Assert
        session.CompletedExercises.Should().BeAssignableTo<IReadOnlyCollection<CompletedExercise?>>();
    }
}
