using Core.Domain.Exceptions;
using FluentAssertions;
using Nutrition.Domain.Entities;
using Nutrition.Domain.ValueObjects;
using Nutrition.UnitTests.Helpers.Builders;

namespace Nutrition.UnitTests.Domain.Entities
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Domain")]
    public class DailyProgressTests
    {
        [Fact]
        public void Constructor_ValidParameters_ShouldCreateDailyProgress()
        {
            // Arrange
            var userId = 1;
            var date = DateOnly.FromDateTime(DateTime.UtcNow);
            var calories = 500;
            var liquids = 1000;

            // Act
            var progress = new DailyProgress(userId, date, calories, liquids);

            // Assert
            progress.UserId.Should().Be(userId);
            progress.Date.Should().Be(date);
            progress.CaloriesConsumed.Should().Be(calories);
            progress.LiquidsConsumedMl.Should().Be(liquids);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Constructor_InvalidUserId_ShouldThrowDomainException(int invalidUserId)
        {
            // Arrange
            var date = DateOnly.FromDateTime(DateTime.UtcNow);

            // Act
            var act = () => new DailyProgress(invalidUserId, date, 0, 0);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Constructor_NegativeCalories_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var date = DateOnly.FromDateTime(DateTime.UtcNow);

            // Act
            var act = () => new DailyProgress(1, date, -1, 0);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Constructor_NegativeLiquids_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var date = DateOnly.FromDateTime(DateTime.UtcNow);

            // Act
            var act = () => new DailyProgress(1, date, 0, -1);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Constructor_PastDate_ShouldThrowDomainException()
        {
            // Arrange
            var pastDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

            // Act
            var act = () => new DailyProgress(1, pastDate, 0, 0);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Constructor_ZeroValues_ShouldCreateDailyProgress()
        {
            // Arrange
            var date = DateOnly.FromDateTime(DateTime.UtcNow);

            // Act
            var progress = new DailyProgress(1, date, 0, 0);

            // Assert
            progress.CaloriesConsumed.Should().Be(0);
            progress.LiquidsConsumedMl.Should().Be(0);
        }

        [Fact]
        public void SetGoal_ValidGoal_ShouldSetGoal()
        {
            // Arrange
            var progress = new DailyProgressBuilder().Build();
            var goal = new DailyGoal(2000, 2500);

            // Act
            progress.SetGoal(goal);

            // Assert
            progress.Goal.Should().Be(goal);
            progress.Goal!.Calories.Should().Be(2000);
            progress.Goal!.QuantityMl.Should().Be(2500);
        }

        [Fact]
        public void SetGoal_NullGoal_ShouldThrowDomainException()
        {
            // Arrange
            var progress = new DailyProgressBuilder().Build();

            // Act
            var act = () => progress.SetGoal(null!);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void ResetGoal_ShouldSetGoalToZero()
        {
            // Arrange
            var progress = new DailyProgressBuilder()
                .WithGoal(new DailyGoal(2000, 2500))
                .Build();

            // Act
            progress.ResetGoal();

            // Assert
            progress.Goal!.Calories.Should().Be(0);
            progress.Goal!.QuantityMl.Should().Be(0);
        }

        [Fact]
        public void SetConsumed_ValidValues_ShouldSetConsumedValues()
        {
            // Arrange
            var progress = new DailyProgressBuilder().Build();

            // Act
            progress.SetConsumed(1500, 2000);

            // Assert
            progress.CaloriesConsumed.Should().Be(1500);
            progress.LiquidsConsumedMl.Should().Be(2000);
        }

        [Fact]
        public void SetConsumed_NegativeCalories_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var progress = new DailyProgressBuilder().Build();

            // Act
            var act = () => progress.SetConsumed(-1, 0);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void SetConsumed_NegativeLiquids_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var progress = new DailyProgressBuilder().Build();

            // Act
            var act = () => progress.SetConsumed(0, -1);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void AddCalories_ValidAmount_ShouldAddToConsumed()
        {
            // Arrange
            var progress = new DailyProgressBuilder()
                .WithCaloriesConsumed(500)
                .Build();

            // Act
            progress.AddCalories(300);

            // Assert
            progress.CaloriesConsumed.Should().Be(800);
        }

        [Fact]
        public void AddCalories_NegativeAmount_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var progress = new DailyProgressBuilder().Build();

            // Act
            var act = () => progress.AddCalories(-1);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void AddLiquidsQuantity_ValidAmount_ShouldAddToConsumed()
        {
            // Arrange
            var progress = new DailyProgressBuilder()
                .WithLiquidsConsumedMl(1000)
                .Build();

            // Act
            progress.AddLiquidsQuantity(500);

            // Assert
            progress.LiquidsConsumedMl.Should().Be(1500);
        }

        [Fact]
        public void AddLiquidsQuantity_NegativeAmount_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var progress = new DailyProgressBuilder().Build();

            // Act
            var act = () => progress.AddLiquidsQuantity(-1);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void GetCaloriesProgressPercentage_GoalZero_ShouldReturnZero()
        {
            // Arrange
            var progress = new DailyProgressBuilder().Build();

            // Act
            var percentage = progress.GetCaloriesProgressPercentage();

            // Assert
            percentage.Should().Be(0);
        }

        [Fact]
        public void GetCaloriesProgressPercentage_HalfConsumed_ShouldReturn50()
        {
            // Arrange
            var progress = new DailyProgressBuilder()
                .WithCaloriesConsumed(1000)
                .WithGoal(new DailyGoal(2000, 0))
                .Build();

            // Act
            var percentage = progress.GetCaloriesProgressPercentage();

            // Assert
            percentage.Should().Be(50);
        }

        [Fact]
        public void GetCaloriesProgressPercentage_ExceedsGoal_ShouldReturnMax100()
        {
            // Arrange
            var progress = new DailyProgressBuilder()
                .WithCaloriesConsumed(3000)
                .WithGoal(new DailyGoal(2000, 0))
                .Build();

            // Act
            var percentage = progress.GetCaloriesProgressPercentage();

            // Assert
            percentage.Should().Be(100);
        }

        [Fact]
        public void GetLiquidsProgressPercentage_GoalZero_ShouldReturnZero()
        {
            // Arrange
            var progress = new DailyProgressBuilder().Build();

            // Act
            var percentage = progress.GetLiquidsProgressPercentage();

            // Assert
            percentage.Should().Be(0);
        }

        [Fact]
        public void GetLiquidsProgressPercentage_HalfConsumed_ShouldReturn50()
        {
            // Arrange
            var progress = new DailyProgressBuilder()
                .WithLiquidsConsumedMl(1250)
                .WithGoal(new DailyGoal(0, 2500))
                .Build();

            // Act
            var percentage = progress.GetLiquidsProgressPercentage();

            // Assert
            percentage.Should().Be(50);
        }

        [Fact]
        public void IsGoalMet_BothGoalsMet_ShouldReturnTrue()
        {
            // Arrange
            var progress = new DailyProgressBuilder()
                .WithCaloriesConsumed(2000)
                .WithLiquidsConsumedMl(2500)
                .WithGoal(new DailyGoal(2000, 2500))
                .Build();

            // Act
            var result = progress.IsGoalMet();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsGoalMet_OnlyCaloriesMet_ShouldReturnFalse()
        {
            // Arrange
            var progress = new DailyProgressBuilder()
                .WithCaloriesConsumed(2000)
                .WithLiquidsConsumedMl(500)
                .WithGoal(new DailyGoal(2000, 2500))
                .Build();

            // Act
            var result = progress.IsGoalMet();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsGoalMet_OnlyLiquidsMet_ShouldReturnFalse()
        {
            // Arrange
            var progress = new DailyProgressBuilder()
                .WithCaloriesConsumed(500)
                .WithLiquidsConsumedMl(2500)
                .WithGoal(new DailyGoal(2000, 2500))
                .Build();

            // Act
            var result = progress.IsGoalMet();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsCaloriesGoalMet_ConsumedEqualsGoal_ShouldReturnTrue()
        {
            // Arrange
            var progress = new DailyProgressBuilder()
                .WithCaloriesConsumed(2000)
                .WithGoal(new DailyGoal(2000, 0))
                .Build();

            // Act
            var result = progress.IsCaloriesGoalMet();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsCaloriesGoalMet_ConsumedLessThanGoal_ShouldReturnFalse()
        {
            // Arrange
            var progress = new DailyProgressBuilder()
                .WithCaloriesConsumed(500)
                .WithGoal(new DailyGoal(2000, 0))
                .Build();

            // Act
            var result = progress.IsCaloriesGoalMet();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsLiquidsGoalMet_ConsumedEqualsGoal_ShouldReturnTrue()
        {
            // Arrange
            var progress = new DailyProgressBuilder()
                .WithLiquidsConsumedMl(2500)
                .WithGoal(new DailyGoal(0, 2500))
                .Build();

            // Act
            var result = progress.IsLiquidsGoalMet();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsLiquidsGoalMet_ConsumedLessThanGoal_ShouldReturnFalse()
        {
            // Arrange
            var progress = new DailyProgressBuilder()
                .WithLiquidsConsumedMl(500)
                .WithGoal(new DailyGoal(0, 2500))
                .Build();

            // Act
            var result = progress.IsLiquidsGoalMet();

            // Assert
            result.Should().BeFalse();
        }
    }
}
