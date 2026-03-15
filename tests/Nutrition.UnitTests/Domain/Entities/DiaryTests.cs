using Core.Domain.Exceptions;
using FluentAssertions;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Events;
using Nutrition.UnitTests.Helpers.Builders;

namespace Nutrition.UnitTests.Domain.Entities
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Domain")]
    public class DiaryTests
    {
        [Fact]
        public void Constructor_ValidParameters_ShouldCreateDiary()
        {
            // Arrange
            var userId = 1;
            var date = DateOnly.FromDateTime(DateTime.UtcNow);

            // Act
            var diary = new Diary(userId, date);

            // Assert
            diary.UserId.Should().Be(userId);
            diary.Date.Should().Be(date);
            diary.Meals.Should().BeEmpty();
            diary.Liquids.Should().BeEmpty();
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
            var act = () => new Diary(invalidUserId, date);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void AddMeal_ValidMeal_ShouldAddMealToCollection()
        {
            // Arrange
            var diary = new DiaryBuilder().Build();
            var meal = new MealBuilder().Build();

            // Act
            diary.AddMeal(meal);

            // Assert
            diary.Meals.Should().ContainSingle();
            diary.Meals.Should().Contain(meal);
        }

        [Fact]
        public void AddMeal_NullMeal_ShouldThrowDomainException()
        {
            // Arrange
            var diary = new DiaryBuilder().Build();

            // Act
            var act = () => diary.AddMeal(null!);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void AddMeal_ValidMeal_ShouldRaiseMealAddedToDiaryEvent()
        {
            // Arrange
            var diary = new DiaryBuilder().Build();
            var meal = new MealBuilder().Build();

            // Act
            diary.AddMeal(meal);

            // Assert
            diary.DomainEvents.Should().ContainSingle()
                .Which.Should().BeOfType<MealAddedToDiaryEvent>();
        }

        [Fact]
        public void RemoveMeal_ValidMeal_ShouldRemoveMealFromCollection()
        {
            // Arrange
            var diary = new DiaryBuilder().Build();
            var meal = new MealBuilder().Build();
            diary.AddMeal(meal);

            // Act
            diary.RemoveMeal(meal);

            // Assert
            diary.Meals.Should().BeEmpty();
        }

        [Fact]
        public void RemoveMeal_NullMeal_ShouldThrowDomainException()
        {
            // Arrange
            var diary = new DiaryBuilder().Build();

            // Act
            var act = () => diary.RemoveMeal(null!);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void RemoveMeal_MealNotInDiary_ShouldThrowDomainException()
        {
            // Arrange
            var diary = new DiaryBuilder().Build();
            var meal = new MealBuilder().Build();

            // Act
            var act = () => diary.RemoveMeal(meal);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void AddLiquid_ValidLiquid_ShouldAddLiquidToCollection()
        {
            // Arrange
            var diary = new DiaryBuilder().Build();
            var liquid = new LiquidBuilder().Build();

            // Act
            diary.AddLiquid(liquid);

            // Assert
            diary.Liquids.Should().ContainSingle();
            diary.Liquids.Should().Contain(liquid);
        }

        [Fact]
        public void AddLiquid_NullLiquid_ShouldThrowDomainException()
        {
            // Arrange
            var diary = new DiaryBuilder().Build();

            // Act
            var act = () => diary.AddLiquid(null!);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void AddLiquid_ValidLiquid_ShouldRaiseLiquidChangedEvent()
        {
            // Arrange
            var diary = new DiaryBuilder().Build();
            var liquid = new LiquidBuilder().Build();

            // Act
            diary.AddLiquid(liquid);

            // Assert
            diary.DomainEvents.Should().ContainSingle()
                .Which.Should().BeOfType<LiquidChangedEvent>();
        }

        [Fact]
        public void RemoveLiquid_ValidLiquid_ShouldRemoveLiquidFromCollection()
        {
            // Arrange
            var diary = new DiaryBuilder().Build();
            var liquid = new LiquidBuilder().Build();
            diary.AddLiquid(liquid);

            // Act
            diary.RemoveLiquid(liquid);

            // Assert
            diary.Liquids.Should().BeEmpty();
        }

        [Fact]
        public void RemoveLiquid_NullLiquid_ShouldThrowDomainException()
        {
            // Arrange
            var diary = new DiaryBuilder().Build();

            // Act
            var act = () => diary.RemoveLiquid(null!);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void RemoveLiquid_LiquidNotInDiary_ShouldThrowDomainException()
        {
            // Arrange
            var diary = new DiaryBuilder().Build();
            var liquid = new LiquidBuilder().Build();

            // Act
            var act = () => diary.RemoveLiquid(liquid);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void RemoveLiquid_ValidLiquid_ShouldRaiseLiquidChangedEvent()
        {
            // Arrange
            var diary = new DiaryBuilder().Build();
            var liquid = new LiquidBuilder().Build();
            diary.AddLiquid(liquid);
            diary.ClearDomainEvents();

            // Act
            diary.RemoveLiquid(liquid);

            // Assert
            diary.DomainEvents.Should().ContainSingle()
                .Which.Should().BeOfType<LiquidChangedEvent>();
        }

        [Fact]
        public void UpdateDate_ValidDate_ShouldUpdateDate()
        {
            // Arrange
            var diary = new DiaryBuilder().Build();
            var newDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            // Act
            diary.UpdateDate(newDate);

            // Assert
            diary.Date.Should().Be(newDate);
            diary.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void TotalCalories_WithNoMeals_ShouldReturnZero()
        {
            // Arrange
            var diary = new DiaryBuilder().Build();

            // Act
            var totalCalories = diary.TotalCalories;

            // Assert
            totalCalories.Should().Be(0);
        }

        [Fact]
        public void TotalLiquidsMl_WithNoLiquids_ShouldReturnZero()
        {
            // Arrange
            var diary = new DiaryBuilder().Build();

            // Act
            var totalLiquids = diary.TotalLiquidsMl;

            // Assert
            totalLiquids.Should().Be(0);
        }

        [Fact]
        public void TotalLiquidsMl_WithLiquids_ShouldReturnSum()
        {
            // Arrange
            var diary = new DiaryBuilder().Build();
            var liquid1 = new LiquidBuilder().WithQuantity(250).Build();
            var liquid2 = new LiquidBuilder().WithQuantity(500).Build();
            diary.AddLiquid(liquid1);
            diary.AddLiquid(liquid2);

            // Act
            var totalLiquids = diary.TotalLiquidsMl;

            // Assert
            totalLiquids.Should().Be(750);
        }
    }
}
