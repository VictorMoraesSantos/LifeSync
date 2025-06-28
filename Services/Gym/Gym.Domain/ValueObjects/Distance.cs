using Core.Domain.Exceptions;
using Core.Domain.ValueObjects;
using Gym.Domain.Enums;

namespace Gym.Domain.ValueObjects
{
    public class Distance : ValueObject
    {
        public decimal Value { get; private set; }
        public MeasurementUnit Unit { get; private set; }

        protected Distance() { }

        public Distance(decimal value, MeasurementUnit unit)
        {
            if (value < 0)
                throw new DomainException("Distance cannot be negative");

            if (unit != MeasurementUnit.Meter && unit != MeasurementUnit.Kilometer && unit != MeasurementUnit.Mile)
                throw new DomainException("Distance unit must be meter, kilometer, or mile");

            Value = value;
            Unit = unit;
        }

        public Distance ConvertTo(MeasurementUnit targetUnit)
        {
            if (Unit == targetUnit)
                return this;

            decimal convertedValue;

            if (Unit == MeasurementUnit.Meter && targetUnit == MeasurementUnit.Kilometer)
            {
                convertedValue = Value / 1000m;
            }
            else if (Unit == MeasurementUnit.Kilometer && targetUnit == MeasurementUnit.Meter)
            {
                convertedValue = Value * 1000m;
            }
            else if (Unit == MeasurementUnit.Kilometer && targetUnit == MeasurementUnit.Mile)
            {
                convertedValue = Value * 0.621371m;
            }
            else if (Unit == MeasurementUnit.Mile && targetUnit == MeasurementUnit.Kilometer)
            {
                convertedValue = Value * 1.60934m;
            }
            else if (Unit == MeasurementUnit.Meter && targetUnit == MeasurementUnit.Mile)
            {
                convertedValue = Value * 0.000621371m;
            }
            else if (Unit == MeasurementUnit.Mile && targetUnit == MeasurementUnit.Meter)
            {
                convertedValue = Value * 1609.34m;
            }
            else
            {
                throw new DomainException($"Cannot convert from {Unit} to {targetUnit}");
            }

            return new Distance(convertedValue, targetUnit);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return Unit;
        }
    }
}
