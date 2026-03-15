using FluentAssertions;
using Gym.Domain.ValueObjects;

namespace Gym.UnitTests.Domain.ValueObjects;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class RestTimeTests
{
    [Fact]
    public void Create_WithValidValue_ShouldCreateRestTime()
    {
        // Arrange & Act
        var restTime = RestTime.Create(60);

        // Assert
        restTime.Should().NotBeNull();
        restTime.Value.Should().Be(60);
    }

    [Fact]
    public void Create_WithZero_ShouldCreateRestTime()
    {
        // Arrange & Act
        var restTime = RestTime.Create(0);

        // Assert
        restTime.Value.Should().Be(0);
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var action = () => RestTime.Create(-1);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithLargeValue_ShouldCreateRestTime()
    {
        // Arrange & Act
        var restTime = RestTime.Create(300);

        // Assert
        restTime.Value.Should().Be(300);
    }
}
