using Core.Domain.ValueObjects;

namespace Gym.Domain.ValueObjects
{
    public sealed class ExerciseIntensity : ValueObject
    {
        public int Value { get; private set; }

        private ExerciseIntensity() { }

        private ExerciseIntensity(int value)
        {
            Value = value;
        }

        public static ExerciseIntensity Create(int intensity)
        {
            if (intensity < 1 || intensity > 10)
                throw new ArgumentException("Intensidade deve estar entre 1 e 10");

            return new ExerciseIntensity(intensity);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
