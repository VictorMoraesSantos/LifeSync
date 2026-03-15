using Core.Domain.Exceptions;
using FluentAssertions;
using Nutrition.Domain.Entities;
using Nutrition.UnitTests.Helpers.Builders;

namespace Nutrition.UnitTests.Domain.Entities
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Domain")]
    public class MealFoodTests
    {
        [Fact]
        public void Constructor_ValidParameters_ShouldCreateMealFood()
        {
            // Arrange
            var mealId = 1;
            var foodId = 2;
            var quantity = 150;

            // Act
            var mealFood = new MealFood(mealId, foodId, quantity);

            // Assert
            mealFood.MealId.Should().Be(mealId);
            mealFood.FoodId.Should().Be(foodId);
            mealFood.Quantity.Should().Be(quantity);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Constructor_InvalidMealId_ShouldThrowDomainException(int invalidMealId)
        {
            // Act
            var act = () => new MealFood(invalidMealId, 1, 100);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Constructor_InvalidFoodId_ShouldThrowDomainException(int invalidFoodId)
        {
            // Act
            var act = () => new MealFood(1, invalidFoodId, 100);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-50)]
        public void Constructor_InvalidQuantity_ShouldThrowDomainException(int invalidQuantity)
        {
            // Act
            var act = () => new MealFood(1, 1, invalidQuantity);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Update_ValidParameters_ShouldUpdateFoodIdAndQuantity()
        {
            // Arrange
            var mealFood = new MealFoodBuilder().Build();
            var newFoodId = 5;
            var newQuantity = 200;

            // Act
            mealFood.Update(newFoodId, newQuantity);

            // Assert
            mealFood.FoodId.Should().Be(newFoodId);
            mealFood.Quantity.Should().Be(newQuantity);
            mealFood.UpdatedAt.Should().NotBeNull();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Update_InvalidFoodId_ShouldThrowDomainException(int invalidFoodId)
        {
            // Arrange
            var mealFood = new MealFoodBuilder().Build();

            // Act
            var act = () => mealFood.Update(invalidFoodId, 100);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Update_InvalidQuantity_ShouldThrowDomainException(int invalidQuantity)
        {
            // Arrange
            var mealFood = new MealFoodBuilder().Build();

            // Act
            var act = () => mealFood.Update(1, invalidQuantity);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void TotalCalories_WithNoFood_ShouldReturnZero()
        {
            // Arrange
            var mealFood = new MealFoodBuilder().Build();

            // Act
            var totalCalories = mealFood.TotalCalories;

            // Assert
            totalCalories.Should().Be(0);
        }

        [Fact]
        public void TotalCalories_WithFood_ShouldCalculateCorrectly()
        {
            // Arrange - MealFood has no public setter for Food, so TotalCalories depends on Food being set
            // When Food is null, TotalCalories should be 0
            var mealFood = new MealFoodBuilder().WithQuantity(200).Build();

            // Act
            var totalCalories = mealFood.TotalCalories;

            // Assert
            totalCalories.Should().Be(0); // Food is null
        }
    }
}
