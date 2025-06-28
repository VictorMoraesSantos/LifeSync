using Core.Domain.Exceptions;
using Core.Domain.ValueObjects;
using Gym.Domain.Enums;

namespace Gym.Domain.ValueObjects
{
    public class Weight : ValueObject
    {
        public decimal Value { get; private set; }
        public MeasurementUnit Unit { get; private set; }

        protected Weight() { }

        public Weight(decimal value, MeasurementUnit unit)
        {
            if (value < 0)
                throw new DomainException("Weight cannot be negative");

            if (unit != MeasurementUnit.Kilogram && unit != MeasurementUnit.Pound)
                throw new DomainException("Weight unit must be kilogram or pound");

            Value = value;
            Unit = unit;
        }

        public Weight ConvertTo(MeasurementUnit targetUnit)
        {
            if (Unit == targetUnit)
                return this;

            decimal convertedValue;

            if (Unit == MeasurementUnit.Kilogram && targetUnit == MeasurementUnit.Pound)
            {
                convertedValue = Value * 2.20462m;
            }
            else if (Unit == MeasurementUnit.Pound && targetUnit == MeasurementUnit.Kilogram)
            {
                convertedValue = Value * 0.453592m;
            }
            else
            {
                throw new DomainException($"Cannot convert from {Unit} to {targetUnit}");
            }

            return new Weight(convertedValue, targetUnit);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return Unit;
        }
    }
}
