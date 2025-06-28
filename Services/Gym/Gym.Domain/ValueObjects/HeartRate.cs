using Core.Domain.Exceptions;
using Core.Domain.ValueObjects;

namespace Gym.Domain.ValueObjects
{
    public class HeartRate : ValueObject
    {
        public int BeatsPerMinute { get; private set; }

        protected HeartRate() { }

        public HeartRate(int beatsPerMinute)
        {
            if (beatsPerMinute <= 0)
                throw new DomainException("Heart rate must be greater than zero");

            if (beatsPerMinute > 250)
                throw new DomainException("Heart rate is unrealistically high");

            BeatsPerMinute = beatsPerMinute;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return BeatsPerMinute;
        }
    }
}
