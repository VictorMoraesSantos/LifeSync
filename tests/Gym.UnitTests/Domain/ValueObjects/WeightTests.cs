using FluentAssertions;
using Gym.Domain.Enums;
using Gym.Domain.ValueObjects;

namespace Gym.UnitTests.Domain.ValueObjects;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class WeightTests
{
    [Fact]
    public void Create_WithValidValue_ShouldCreateWeight()
    {
        // Arrange & Act
        var weight = Weight.Create(80, MeasurementUnit.Kilogram);

        // Assert
        weight.Should().NotBeNull();
        weight.Value.Should().Be(80);
        weight.Unit.Should().Be(MeasurementUnit.Kilogram);
    }

    [Fact]
    public void Create_WithZeroValue_ShouldCreateWeight()
    {
        // Arrange & Act
        var weight = Weight.Create(0, MeasurementUnit.Kilogram);

        // Assert
        weight.Value.Should().Be(0);
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var action = () => Weight.Create(-1, MeasurementUnit.Kilogram);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithPounds_ShouldCreateWeightInPounds()
    {
        // Arrange & Act
        var weight = Weight.Create(175, MeasurementUnit.Pound);

        // Assert
        weight.Value.Should().Be(175);
        weight.Unit.Should().Be(MeasurementUnit.Pound);
    }

    [Fact]
    public void Equals_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var weight1 = Weight.Create(80, MeasurementUnit.Kilogram);
        var weight2 = Weight.Create(80, MeasurementUnit.Kilogram);

        // Assert
        weight1.Should().Be(weight2);
    }

    [Fact]
    public void Equals_WithDifferentUnits_ShouldNotBeEqual()
    {
        // Arrange
        var weightKg = Weight.Create(80, MeasurementUnit.Kilogram);
        var weightLb = Weight.Create(80, MeasurementUnit.Pound);

        // Assert
        weightKg.Should().NotBe(weightLb);
    }
}
