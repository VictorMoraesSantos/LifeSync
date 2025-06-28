using Core.Domain.Exceptions;
using Core.Domain.ValueObjects;

namespace Gym.Domain.ValueObjects
{
    public class CardioExecution : ValueObject
    {
        public Distance Distance { get; private set; }
        public Duration Duration { get; private set; }
        public Pace AveragePace { get; private set; }
        public HeartRate AverageHeartRate { get; private set; }
        public HeartRate MaxHeartRate { get; private set; }

        protected CardioExecution() { }

        public CardioExecution(
            Duration duration,
            Distance distance = null,
            Pace averagePace = null,
            HeartRate averageHeartRate = null,
            HeartRate maxHeartRate = null)
        {
            if (duration == null && distance == null)
                throw new DomainException("Either duration or distance must be provided");

            Duration = duration;
            Distance = distance;
            AveragePace = averagePace;
            AverageHeartRate = averageHeartRate;
            MaxHeartRate = maxHeartRate;

            ValidateHeartRates();
        }

        private void ValidateHeartRates()
        {
            if (AverageHeartRate != null && MaxHeartRate != null)
            {
                if (AverageHeartRate.BeatsPerMinute > MaxHeartRate.BeatsPerMinute)
                    throw new DomainException("Average heart rate cannot be greater than max heart rate");
            }
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Distance;
            yield return Duration;
            yield return AveragePace;
            yield return AverageHeartRate;
            yield return MaxHeartRate;
        }
    }
}
