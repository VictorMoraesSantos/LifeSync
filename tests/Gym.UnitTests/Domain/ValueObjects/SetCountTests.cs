using FluentAssertions;
using Gym.Domain.ValueObjects;

namespace Gym.UnitTests.Domain.ValueObjects;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class SetCountTests
{
    [Fact]
    public void Create_WithValidValue_ShouldCreateSetCount()
    {
        // Arrange & Act
        var setCount = SetCount.Create(3);

        // Assert
        setCount.Should().NotBeNull();
        setCount.Value.Should().Be(3);
    }

    [Fact]
    public void Create_WithOne_ShouldCreateSetCount()
    {
        // Arrange & Act
        var setCount = SetCount.Create(1);

        // Assert
        setCount.Value.Should().Be(1);
    }

    [Fact]
    public void Create_WithZero_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var action = () => SetCount.Create(0);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var action = () => SetCount.Create(-1);

        // Assert
        action.Should().Throw<ArgumentException>();
    }
}
