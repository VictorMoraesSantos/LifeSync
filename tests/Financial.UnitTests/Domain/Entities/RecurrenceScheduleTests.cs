using Core.Domain.Exceptions;
using Financial.Domain.Entities;
using Financial.Domain.Enums;
using Financial.Domain.Errors;
using Financial.UnitTests.Helpers.Builders;
using FluentAssertions;

namespace Financial.UnitTests.Domain.Entities
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Domain")]
    public class RecurrenceScheduleTests
    {
        #region Constructor Tests

        [Fact]
        public void Create_WithValidParameters_ShouldCreateEntity()
        {
            // Arrange
            var transactionId = 1;
            var frequency = RecurrenceFrequency.Monthly;
            var startDate = DateTime.UtcNow;

            // Act
            var schedule = new RecurrenceSchedule(transactionId, frequency, startDate);

            // Assert
            schedule.Should().NotBeNull();
            schedule.TransactionId.Should().Be(transactionId);
            schedule.Frequency.Should().Be(frequency);
            schedule.StartDate.Should().Be(startDate);
            schedule.EndDate.Should().BeNull();
            schedule.MaxOccurrences.Should().BeNull();
            schedule.OccurrencesGenerated.Should().Be(0);
        }

        [Fact]
        public void Create_WithEndDateAndMaxOccurrences_ShouldSetOptionalFields()
        {
            // Arrange
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddMonths(6);
            var maxOccurrences = 10;

            // Act
            var schedule = new RecurrenceSchedule(1, RecurrenceFrequency.Weekly, startDate, endDate, maxOccurrences);

            // Assert
            schedule.EndDate.Should().Be(endDate);
            schedule.MaxOccurrences.Should().Be(maxOccurrences);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Create_WithInvalidTransactionId_ShouldThrowArgumentOutOfRangeException(int invalidId)
        {
            // Act
            var act = () => new RecurrenceSchedule(invalidId, RecurrenceFrequency.Monthly, DateTime.UtcNow);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Create_WithEndDateBeforeStartDate_ShouldThrowDomainException()
        {
            // Arrange
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(-1);

            // Act
            var act = () => new RecurrenceSchedule(1, RecurrenceFrequency.Monthly, startDate, endDate);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage(RecurrenceScheduleErrors.EndDateBeforeStartDate.Description);
        }

        [Fact]
        public void Create_WithEndDateEqualToStartDate_ShouldThrowDomainException()
        {
            // Arrange
            var startDate = DateTime.UtcNow;

            // Act
            var act = () => new RecurrenceSchedule(1, RecurrenceFrequency.Monthly, startDate, startDate);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage(RecurrenceScheduleErrors.EndDateBeforeStartDate.Description);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Create_WithInvalidMaxOccurrences_ShouldThrowDomainException(int invalidMax)
        {
            // Act
            var act = () => new RecurrenceSchedule(1, RecurrenceFrequency.Monthly, DateTime.UtcNow, null, invalidMax);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage(RecurrenceScheduleErrors.InvalidMaxOccurrences.Description);
        }

        #endregion

        #region Update Tests

        [Fact]
        public void Update_WithValidParameters_ShouldUpdateProperties()
        {
            // Arrange
            var schedule = RecurrenceScheduleBuilder.Default().Build();
            var newFrequency = RecurrenceFrequency.Weekly;
            var newEndDate = DateTime.UtcNow.AddYears(1);
            var newMaxOccurrences = 5;

            // Act
            schedule.Update(newFrequency, newEndDate, newMaxOccurrences);

            // Assert
            schedule.Frequency.Should().Be(newFrequency);
            schedule.EndDate.Should().Be(newEndDate);
            schedule.MaxOccurrences.Should().Be(newMaxOccurrences);
            schedule.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Update_WithEndDateBeforeStartDate_ShouldThrowDomainException()
        {
            // Arrange
            var startDate = DateTime.UtcNow;
            var schedule = RecurrenceScheduleBuilder.Default()
                .WithStartDate(startDate)
                .Build();

            // Act
            var act = () => schedule.Update(RecurrenceFrequency.Daily, startDate.AddDays(-1));

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage(RecurrenceScheduleErrors.EndDateBeforeStartDate.Description);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Update_WithInvalidMaxOccurrences_ShouldThrowDomainException(int invalidMax)
        {
            // Arrange
            var schedule = RecurrenceScheduleBuilder.Default().Build();

            // Act
            var act = () => schedule.Update(RecurrenceFrequency.Daily, null, invalidMax);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage(RecurrenceScheduleErrors.InvalidMaxOccurrences.Description);
        }

        [Fact]
        public void Update_ShouldMarkAsUpdated()
        {
            // Arrange
            var schedule = RecurrenceScheduleBuilder.Default().Build();
            var createdAt = schedule.CreatedAt;

            // Act
            schedule.Update(RecurrenceFrequency.Weekly);

            // Assert
            schedule.UpdatedAt.Should().NotBeNull();
            schedule.UpdatedAt.Should().BeAfter(createdAt);
        }

        #endregion

        #region Activate/Deactivate Tests

        [Fact]
        public void Deactivate_ShouldSetIsActiveFalse()
        {
            // Arrange
            var schedule = RecurrenceScheduleBuilder.Default().Build();

            // Act
            schedule.Deactivate();

            // Assert
            schedule.IsActive.Should().BeFalse();
            schedule.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Activate_ShouldSetIsActiveTrue()
        {
            // Arrange
            var schedule = RecurrenceScheduleBuilder.Default().Build();
            schedule.Deactivate();

            // Act
            schedule.Activate();

            // Assert
            schedule.IsActive.Should().BeTrue();
            schedule.UpdatedAt.Should().NotBeNull();
        }

        #endregion

        #region CanGenerateNext Tests

        [Fact]
        public void CanGenerateNext_WhenInactive_ShouldReturnFalse()
        {
            // Arrange
            var schedule = RecurrenceScheduleBuilder.Default().Build();
            schedule.Deactivate();

            // Act & Assert
            schedule.CanGenerateNext().Should().BeFalse();
        }

        [Fact]
        public void CanGenerateNext_WhenMaxOccurrencesReached_ShouldReturnFalse()
        {
            // Arrange
            var schedule = RecurrenceScheduleBuilder.Default()
                .WithMaxOccurrences(1)
                .Build();
            schedule.Activate();

            // Simulate reaching max by using reflection
            var field = typeof(RecurrenceSchedule).GetProperty("OccurrencesGenerated");
            // OccurrencesGenerated is private set, so we use reflection
            typeof(RecurrenceSchedule)
                .GetProperty("OccurrencesGenerated")!
                .SetValue(schedule, 1);

            // Act & Assert
            schedule.CanGenerateNext().Should().BeFalse();
        }

        [Fact]
        public void CanGenerateNext_WhenNextOccurrencePastEndDate_ShouldReturnFalse()
        {
            // Arrange - NextOccurrence defaults to default(DateTime) which is DateTime.MinValue
            // We need an endDate that is before NextOccurrence to trigger the condition
            // Since NextOccurrence is not publicly set, we need a schedule where endDate < NextOccurrence
            // The entity's NextOccurrence is not set in the constructor, so it stays at default(DateTime)
            // To properly test this, we use a schedule with endDate in the past relative to NextOccurrence
            var startDate = DateTime.UtcNow.AddDays(-30);
            var endDate = startDate.AddDays(1); // endDate is after startDate (valid)
            var schedule = RecurrenceScheduleBuilder.Default()
                .WithStartDate(startDate)
                .WithEndDate(endDate)
                .Build();

            // Set NextOccurrence to after endDate via reflection to trigger the condition
            typeof(RecurrenceSchedule)
                .GetProperty("NextOccurrence")!
                .SetValue(schedule, endDate.AddDays(1));

            // Ensure the schedule is active via reflection (Activate() now validates conditions)
            typeof(RecurrenceSchedule)
                .GetProperty("IsActive")!
                .SetValue(schedule, true);

            // Act & Assert
            schedule.CanGenerateNext().Should().BeFalse();
        }

        #endregion

        #region GenerateTransaction Tests

        [Fact]
        public void GenerateTransaction_WhenInactive_ShouldThrowDomainException()
        {
            // Arrange
            var schedule = RecurrenceScheduleBuilder.Default().Build();
            schedule.Deactivate();

            // Act
            var act = () => schedule.GenerateTransaction();

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage(RecurrenceScheduleErrors.InactiveSchedule.Description);
        }

        [Fact]
        public void GenerateTransaction_WhenTransactionNotLoaded_ShouldThrowDomainException()
        {
            // Arrange
            var schedule = RecurrenceScheduleBuilder.Default()
                .WithMaxOccurrences(5)
                .Build();
            schedule.Activate();

            // Act
            var act = () => schedule.GenerateTransaction();

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage(RecurrenceScheduleErrors.TransactionNotLoaded.Description);
        }

        #endregion

        #region RecurrenceFrequency Tests

        [Theory]
        [InlineData(RecurrenceFrequency.Daily, "Daily")]
        [InlineData(RecurrenceFrequency.Weekly, "Weekly")]
        [InlineData(RecurrenceFrequency.Monthly, "Monthly")]
        [InlineData(RecurrenceFrequency.Yearly, "Yearly")]
        public void RecurrenceFrequency_ToFriendlyString_ShouldReturnCorrectString(RecurrenceFrequency frequency, string expected)
        {
            // Act & Assert
            frequency.ToFriendlyString().Should().Be(expected);
        }

        [Theory]
        [InlineData(RecurrenceFrequency.Daily)]
        [InlineData(RecurrenceFrequency.Weekly)]
        [InlineData(RecurrenceFrequency.Monthly)]
        [InlineData(RecurrenceFrequency.Yearly)]
        public void Create_WithAllFrequencies_ShouldSucceed(RecurrenceFrequency frequency)
        {
            // Act
            var schedule = RecurrenceScheduleBuilder.Default()
                .WithFrequency(frequency)
                .Build();

            // Assert
            schedule.Frequency.Should().Be(frequency);
        }

        #endregion
    }
}
