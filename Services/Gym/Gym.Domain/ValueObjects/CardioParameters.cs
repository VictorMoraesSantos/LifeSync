using Core.Domain.Exceptions;
using Core.Domain.ValueObjects;

namespace Gym.Domain.ValueObjects
{
    public class CardioParameters : ValueObject
    {
        public Distance Distance { get; private set; }
        public Duration Duration { get; private set; }
        public Intensity Intensity { get; private set; }

        protected CardioParameters() { }

        public CardioParameters(Distance distance = null, Duration duration = null, Intensity intensity = null)
        {
            if (distance == null && duration == null)
                throw new DomainException("Either distance or duration must be specified");

            Distance = distance;
            Duration = duration;
            Intensity = intensity;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Distance;
            yield return Duration;
            yield return Intensity;
        }
    }
}
