using Core.Domain.Exceptions;
using FluentAssertions;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Events;
using Nutrition.UnitTests.Helpers.Builders;

namespace Nutrition.UnitTests.Domain.Entities
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Domain")]
    public class MealTests
    {
        [Fact]
        public void Constructor_ValidParameters_ShouldCreateMeal()
        {
            // Arrange
            var name = "Almoço";
            var description = "Refeição do meio-dia";

            // Act
            var meal = new Meal(name, description);

            // Assert
            meal.Name.Should().Be(name);
            meal.Description.Should().Be(description);
            meal.MealFoods.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_InvalidName_ShouldThrowDomainException(string? invalidName)
        {
            // Act
            var act = () => new Meal(invalidName!, "Descrição válida");

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_InvalidDescription_ShouldThrowDomainException(string? invalidDescription)
        {
            // Act
            var act = () => new Meal("Nome válido", invalidDescription!);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void SetDiaryId_ValidId_ShouldSetDiaryId()
        {
            // Arrange
            var meal = new MealBuilder().Build();

            // Act
            meal.SetDiaryId(5);

            // Assert
            meal.DiaryId.Should().Be(5);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void SetDiaryId_InvalidId_ShouldThrowDomainException(int invalidId)
        {
            // Arrange
            var meal = new MealBuilder().Build();

            // Act
            var act = () => meal.SetDiaryId(invalidId);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Update_ValidParameters_ShouldUpdateNameAndDescription()
        {
            // Arrange
            var meal = new MealBuilder().Build();
            var newName = "Jantar";
            var newDescription = "Refeição da noite";

            // Act
            meal.Update(newName, newDescription);

            // Assert
            meal.Name.Should().Be(newName);
            meal.Description.Should().Be(newDescription);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_InvalidName_ShouldThrowDomainException(string? invalidName)
        {
            // Arrange
            var meal = new MealBuilder().Build();

            // Act
            var act = () => meal.Update(invalidName!, "Descrição válida");

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_InvalidDescription_ShouldThrowDomainException(string? invalidDescription)
        {
            // Arrange
            var meal = new MealBuilder().Build();

            // Act
            var act = () => meal.Update("Nome válido", invalidDescription!);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void AddMealFood_ValidMealFood_ShouldAddToCollection()
        {
            // Arrange
            var meal = new MealBuilder().Build();
            var mealFood = new MealFoodBuilder().Build();

            // Act
            meal.AddMealFood(mealFood);

            // Assert
            meal.MealFoods.Should().ContainSingle();
            meal.MealFoods.Should().Contain(mealFood);
        }

        [Fact]
        public void AddMealFood_NullMealFood_ShouldThrowDomainException()
        {
            // Arrange
            var meal = new MealBuilder().Build();

            // Act
            var act = () => meal.AddMealFood(null!);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void AddMealFood_ValidMealFood_ShouldRaiseMealFoodAddedEvent()
        {
            // Arrange
            var meal = new MealBuilder().Build();
            var mealFood = new MealFoodBuilder().Build();

            // Act
            meal.AddMealFood(mealFood);

            // Assert
            meal.DomainEvents.Should().ContainSingle()
                .Which.Should().BeOfType<MealFoodAddedEvent>();
        }

        [Fact]
        public void TotalCalories_WithNoMealFoods_ShouldReturnZero()
        {
            // Arrange
            var meal = new MealBuilder().Build();

            // Act
            var totalCalories = meal.TotalCalories;

            // Assert
            totalCalories.Should().Be(0);
        }
    }
}
