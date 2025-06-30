using BuildingBlocks.Results;
using Core.Domain.ValueObjects;
using Gym.Domain.Enums;

namespace Gym.Domain.ValueObjects
{
    public sealed class Weight : ValueObject
    {
        public int Value { get; private set; }
        public MeasurementUnit Unit { get; private set; } = MeasurementUnit.Kilogram;

        private Weight() { }

        private Weight(int value, MeasurementUnit unit)
        {
            Value = value;
            Unit = unit;
        }

        public static Weight Create(int value, MeasurementUnit unit)
        {
            if (value < 0)
                throw new ArgumentException("Peso deve ser não-negativo");

            return new Weight(value, unit);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return Unit;
        }
    }
}
