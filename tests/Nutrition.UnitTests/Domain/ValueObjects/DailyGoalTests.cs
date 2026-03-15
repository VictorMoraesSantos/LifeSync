using Core.Domain.Exceptions;
using FluentAssertions;
using Nutrition.Domain.ValueObjects;

namespace Nutrition.UnitTests.Domain.ValueObjects
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Domain")]
    public class DailyGoalTests
    {
        [Fact]
        public void Constructor_ValidParameters_ShouldCreateDailyGoal()
        {
            // Arrange
            var calories = 2000;
            var quantityMl = 2500;

            // Act
            var goal = new DailyGoal(calories, quantityMl);

            // Assert
            goal.Calories.Should().Be(calories);
            goal.QuantityMl.Should().Be(quantityMl);
        }

        [Fact]
        public void Constructor_ZeroValues_ShouldCreateDailyGoal()
        {
            // Act
            var goal = new DailyGoal(0, 0);

            // Assert
            goal.Calories.Should().Be(0);
            goal.QuantityMl.Should().Be(0);
        }

        [Fact]
        public void Constructor_NegativeCalories_ShouldThrowDomainException()
        {
            // Act
            var act = () => new DailyGoal(-1, 0);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Constructor_NegativeQuantityMl_ShouldThrowDomainException()
        {
            // Act
            var act = () => new DailyGoal(0, -1);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Constructor_BothNegative_ShouldThrowDomainException()
        {
            // Act
            var act = () => new DailyGoal(-100, -200);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Constructor_LargeValidValues_ShouldCreateDailyGoal()
        {
            // Arrange
            var calories = 10000;
            var quantityMl = 5000;

            // Act
            var goal = new DailyGoal(calories, quantityMl);

            // Assert
            goal.Calories.Should().Be(calories);
            goal.QuantityMl.Should().Be(quantityMl);
        }
    }
}
