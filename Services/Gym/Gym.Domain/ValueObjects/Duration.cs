using Core.Domain.Exceptions;
using Core.Domain.ValueObjects;
using Gym.Domain.Enums;

namespace Gym.Domain.ValueObjects
{
    public class Duration : ValueObject
    {
        public int Value { get; private set; }
        public MeasurementUnit Unit { get; private set; }

        protected Duration() { } // For ORM

        public Duration(int value, MeasurementUnit unit)
        {
            if (value < 0)
                throw new DomainException("Duration cannot be negative");

            if (unit != MeasurementUnit.Seconds && unit != MeasurementUnit.Minutes)
                throw new DomainException("Duration unit must be seconds or minutes");

            Value = value;
            Unit = unit;
        }

        public TimeSpan ToTimeSpan()
        {
            return Unit == MeasurementUnit.Seconds
                ? TimeSpan.FromSeconds(Value)
                : TimeSpan.FromMinutes(Value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return Unit;
        }
    }
}
