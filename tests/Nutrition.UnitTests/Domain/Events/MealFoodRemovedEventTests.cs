using FluentAssertions;
using Nutrition.Domain.Events;

namespace Nutrition.UnitTests.Domain.Events
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Domain")]
    public class MealFoodRemovedEventTests
    {
        [Fact]
        public void Constructor_ValidParameters_ShouldCreateEvent()
        {
            // Arrange
            var diaryId = 3;
            var totalCalories = 200.75m;

            // Act
            var domainEvent = new MealFoodRemovedEvent(diaryId, totalCalories);

            // Assert
            domainEvent.DiaryId.Should().Be(diaryId);
            domainEvent.TotalCalories.Should().Be(totalCalories);
            domainEvent.Id.Should().NotBeEmpty();
            domainEvent.OccuredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Constructor_ShouldGenerateUniqueId()
        {
            // Arrange & Act
            var event1 = new MealFoodRemovedEvent(1, 100m);
            var event2 = new MealFoodRemovedEvent(1, 100m);

            // Assert
            event1.Id.Should().NotBe(event2.Id);
        }
    }
}
