using FluentAssertions;
using Nutrition.Domain.Events;

namespace Nutrition.UnitTests.Domain.Events
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Domain")]
    public class MealAddedToDiaryEventTests
    {
        [Fact]
        public void Constructor_ValidParameters_ShouldCreateEvent()
        {
            // Arrange
            var userId = 1;
            var date = DateOnly.FromDateTime(DateTime.UtcNow);
            var mealId = 5;

            // Act
            var domainEvent = new MealAddedToDiaryEvent(userId, date, mealId);

            // Assert
            domainEvent.UserId.Should().Be(userId);
            domainEvent.Date.Should().Be(date);
            domainEvent.MealId.Should().Be(mealId);
            domainEvent.Id.Should().NotBeEmpty();
            domainEvent.OccuredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Constructor_ShouldGenerateUniqueId()
        {
            // Arrange & Act
            var event1 = new MealAddedToDiaryEvent(1, DateOnly.FromDateTime(DateTime.UtcNow), 1);
            var event2 = new MealAddedToDiaryEvent(1, DateOnly.FromDateTime(DateTime.UtcNow), 1);

            // Assert
            event1.Id.Should().NotBe(event2.Id);
        }
    }
}
