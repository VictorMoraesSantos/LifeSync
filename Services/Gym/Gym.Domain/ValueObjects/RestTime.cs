using Core.Domain.ValueObjects;

namespace Gym.Domain.ValueObjects
{
    public sealed class RestTime : ValueObject
    {
        public int Value { get; private set; } // Tempo em segundos

        protected RestTime() { }

        private RestTime(int value)
        {
            Value = value;
        }

        public static RestTime Create(int seconds)
        {
            if (seconds < 0)
                throw new ArgumentException("Tempo de descanso deve ser não-negativo");

            return new RestTime(seconds);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
