using Core.Domain.Exceptions;
using Core.Domain.ValueObjects;
using Gym.Domain.Enums;

namespace Gym.Domain.ValueObjects
{
    public class Duration : ValueObject
    {
        public int Value { get; private set; }
        public MeasurementUnit Unit { get; private set; }

        protected Duration() { }

        public Duration(int value, MeasurementUnit unit)
        {
            if (value < 0)
                throw new DomainException("Duration cannot be negative");

            if (unit != MeasurementUnit.Second && unit != MeasurementUnit.Minute && unit != MeasurementUnit.Hour)
                throw new DomainException("Duration unit must be second, minute, or hour");

            Value = value;
            Unit = unit;
        }

        public TimeSpan ToTimeSpan()
        {
            switch (Unit)
            {
                case MeasurementUnit.Second:
                    return TimeSpan.FromSeconds(Value);
                case MeasurementUnit.Minute:
                    return TimeSpan.FromMinutes(Value);
                case MeasurementUnit.Hour:
                    return TimeSpan.FromHours(Value);
                default:
                    throw new DomainException($"Cannot convert {Unit} to TimeSpan");
            }
        }

        public Duration ConvertTo(MeasurementUnit targetUnit)
        {
            if (Unit == targetUnit)
                return this;

            var timeSpan = ToTimeSpan();
            int convertedValue;

            switch (targetUnit)
            {
                case MeasurementUnit.Second:
                    convertedValue = (int)timeSpan.TotalSeconds;
                    break;
                case MeasurementUnit.Minute:
                    convertedValue = (int)timeSpan.TotalMinutes;
                    break;
                case MeasurementUnit.Hour:
                    convertedValue = (int)timeSpan.TotalHours;
                    break;
                default:
                    throw new DomainException($"Cannot convert to {targetUnit}");
            }

            return new Duration(convertedValue, targetUnit);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return Unit;
        }
    }
}
