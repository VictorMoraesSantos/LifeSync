using BuildingBlocks.Results;
using Core.Domain.ValueObjects;

namespace Gym.Domain.ValueObjects
{
    public sealed class SetCount : ValueObject
    {
        public int Value { get; private set; }

        private SetCount() { }

        private SetCount(int value)
        {
            Value = value;
        }

        public static SetCount Create(int count)
        {
            if (count < 1)
                throw new ArgumentException("Número de repetições deve ser positivo");

            return new SetCount(count);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
