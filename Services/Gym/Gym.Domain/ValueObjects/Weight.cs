using Core.Domain.Exceptions;
using Core.Domain.ValueObjects;
using Gym.Domain.Enums;

namespace Gym.Domain.ValueObjects
{
    public class Weight : ValueObject
    {
        public decimal Value { get; private set; }
        public MeasurementUnit Unit { get; private set; }

        protected Weight() { } // For ORM

        public Weight(decimal value, MeasurementUnit unit)
        {
            if (value < 0)
                throw new DomainException("Weight cannot be negative");

            if (unit != MeasurementUnit.Kilograms && unit != MeasurementUnit.Pounds)
                throw new DomainException("Weight unit must be kilograms or pounds");

            Value = value;
            Unit = unit;
        }

        public Weight ConvertTo(MeasurementUnit targetUnit)
        {
            if (Unit == targetUnit)
                return this;

            if (targetUnit == MeasurementUnit.Kilograms && Unit == MeasurementUnit.Pounds)
                return new Weight(Value * 0.453592m, MeasurementUnit.Kilograms);

            if (targetUnit == MeasurementUnit.Pounds && Unit == MeasurementUnit.Kilograms)
                return new Weight(Value * 2.20462m, MeasurementUnit.Pounds);

            throw new DomainException("Cannot convert between these units");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return Unit;
        }
    }
}
