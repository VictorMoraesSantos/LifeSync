using Financial.Domain.Entities;
using Financial.Domain.Enums;

namespace Financial.UnitTests.Helpers.Builders;

public class RecurrenceScheduleBuilder
{
    private int _transactionId = 1;
    private RecurrenceFrequency _frequency = RecurrenceFrequency.Monthly;
    private DateTime _startDate = DateTime.UtcNow;
    private DateTime? _endDate = null;
    private int? _maxOccurrences = null;

    public RecurrenceScheduleBuilder WithTransactionId(int transactionId)
    {
        _transactionId = transactionId;
        return this;
    }

    public RecurrenceScheduleBuilder WithFrequency(RecurrenceFrequency frequency)
    {
        _frequency = frequency;
        return this;
    }

    public RecurrenceScheduleBuilder WithStartDate(DateTime startDate)
    {
        _startDate = startDate;
        return this;
    }

    public RecurrenceScheduleBuilder WithEndDate(DateTime? endDate)
    {
        _endDate = endDate;
        return this;
    }

    public RecurrenceScheduleBuilder WithMaxOccurrences(int? maxOccurrences)
    {
        _maxOccurrences = maxOccurrences;
        return this;
    }

    public RecurrenceSchedule Build()
    {
        return new RecurrenceSchedule(_transactionId, _frequency, _startDate, _endDate, _maxOccurrences);
    }

    public static RecurrenceScheduleBuilder Default() => new();
}
