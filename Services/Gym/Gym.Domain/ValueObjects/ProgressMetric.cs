using Core.Domain.Exceptions;
using Core.Domain.ValueObjects;
using Gym.Domain.Enums;

namespace Gym.Domain.ValueObjects
{
    public class ProgressMetric : ValueObject
    {
        public string Name { get; private set; }
        public decimal Value { get; private set; }
        public MeasurementUnit Unit { get; private set; }
        public string Notes { get; private set; }

        protected ProgressMetric() { }

        public ProgressMetric(string name, decimal value, MeasurementUnit unit, string notes = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Metric name cannot be empty");

            Name = name;
            Value = value;
            Unit = unit;
            Notes = notes ?? "";
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name.ToLowerInvariant();
            yield return Value;
            yield return Unit;
        }
    }
}
