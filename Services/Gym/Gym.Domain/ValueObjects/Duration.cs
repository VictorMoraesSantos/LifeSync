using Core.Domain.ValueObjects;

namespace Gym.Domain.ValueObjects
{
    public sealed class Duration : ValueObject
    {
        public TimeSpan Value { get; private set; }

        private Duration() { }

        private Duration(TimeSpan value)
        {
            Value = value;
        }

        public static Duration Create(TimeSpan duration)
        {
            if (duration <= TimeSpan.Zero)
                throw new ArgumentException("Duração deve ser positiva");

            return new Duration(duration);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
