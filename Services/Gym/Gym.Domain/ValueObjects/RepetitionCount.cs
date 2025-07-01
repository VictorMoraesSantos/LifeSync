using Core.Domain.ValueObjects;

namespace Gym.Domain.ValueObjects
{
    public sealed class RepetitionCount : ValueObject
    {
        public int Value { get; private set; }

        private RepetitionCount() { }

        private RepetitionCount(int value)
        {
            Value = value;
        }

        public static RepetitionCount Create(int count)
        {
            if (count < 1)
                throw new ArgumentException("Número de repetições deve ser positivo");

            return new RepetitionCount(count);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
