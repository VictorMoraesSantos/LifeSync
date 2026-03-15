using FluentAssertions;
using Gym.Domain.ValueObjects;

namespace Gym.UnitTests.Domain.ValueObjects;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class RepetitionCountTests
{
    [Fact]
    public void Create_WithValidValue_ShouldCreateRepetitionCount()
    {
        // Arrange & Act
        var repCount = RepetitionCount.Create(10);

        // Assert
        repCount.Should().NotBeNull();
        repCount.Value.Should().Be(10);
    }

    [Fact]
    public void Create_WithOne_ShouldCreateRepetitionCount()
    {
        // Arrange & Act
        var repCount = RepetitionCount.Create(1);

        // Assert
        repCount.Value.Should().Be(1);
    }

    [Fact]
    public void Create_WithZero_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var action = () => RepetitionCount.Create(0);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var action = () => RepetitionCount.Create(-1);

        // Assert
        action.Should().Throw<ArgumentException>();
    }
}
