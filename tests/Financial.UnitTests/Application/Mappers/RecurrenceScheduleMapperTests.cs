using Financial.Application.DTOs.RecurrenceSchedule;
using Financial.Application.Mappings;
using Financial.Domain.Entities;
using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;
using FluentAssertions;

namespace Financial.UnitTests.Application.Mappers
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Application")]
    public class RecurrenceScheduleMapperTests
    {
        [Fact]
        public void ToEntity_WithValidDTO_ShouldMapCorrectly()
        {
            // Arrange
            var dto = new CreateRecurrenceScheduleDTO(
                TransactionId: 1,
                Frequency: RecurrenceFrequency.Monthly,
                StartDate: DateTime.UtcNow,
                EndDate: DateTime.UtcNow.AddMonths(6),
                MaxOccurrences: 10);

            // Act
            var entity = RecurrencyScheduleMapper.ToEntity(dto);

            // Assert
            entity.Should().NotBeNull();
            entity.TransactionId.Should().Be(dto.TransactionId);
            entity.Frequency.Should().Be(dto.Frequency);
            entity.StartDate.Should().Be(dto.StartDate);
            entity.EndDate.Should().Be(dto.EndDate);
            entity.MaxOccurrences.Should().Be(dto.MaxOccurrences);
        }

        [Fact]
        public void ToEntity_WithoutOptionalFields_ShouldMapCorrectly()
        {
            // Arrange
            var dto = new CreateRecurrenceScheduleDTO(
                TransactionId: 1,
                Frequency: RecurrenceFrequency.Daily,
                StartDate: DateTime.UtcNow);

            // Act
            var entity = RecurrencyScheduleMapper.ToEntity(dto);

            // Assert
            entity.Should().NotBeNull();
            entity.TransactionId.Should().Be(dto.TransactionId);
            entity.Frequency.Should().Be(dto.Frequency);
            entity.EndDate.Should().BeNull();
            entity.MaxOccurrences.Should().BeNull();
        }

        [Theory]
        [InlineData(RecurrenceFrequency.Daily)]
        [InlineData(RecurrenceFrequency.Weekly)]
        [InlineData(RecurrenceFrequency.Monthly)]
        [InlineData(RecurrenceFrequency.Yearly)]
        public void ToEntity_WithAllFrequencies_ShouldMapCorrectly(RecurrenceFrequency frequency)
        {
            // Arrange
            var dto = new CreateRecurrenceScheduleDTO(1, frequency, DateTime.UtcNow);

            // Act
            var entity = RecurrencyScheduleMapper.ToEntity(dto);

            // Assert
            entity.Frequency.Should().Be(frequency);
        }
    }
}
