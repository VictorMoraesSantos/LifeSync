using Core.Domain.Exceptions;
using Core.Domain.ValueObjects;
using Gym.Domain.Enums;

namespace Gym.Domain.ValueObjects
{
    public class Pace : ValueObject
    {
        public TimeSpan TimePerUnit { get; private set; }
        public MeasurementUnit DistanceUnit { get; private set; }

        protected Pace() { }

        public Pace(TimeSpan timePerUnit, MeasurementUnit distanceUnit)
        {
            if (timePerUnit.TotalSeconds < 0)
                throw new DomainException("Pace time cannot be negative");

            if (distanceUnit != MeasurementUnit.Kilometer && distanceUnit != MeasurementUnit.Mile)
                throw new DomainException("Pace distance unit must be kilometer or mile");

            TimePerUnit = timePerUnit;
            DistanceUnit = distanceUnit;
        }

        // Cria um pace a partir de uma distância e uma duração
        public static Pace FromDistanceAndDuration(Distance distance, Duration duration)
        {
            if (distance == null)
                throw new DomainException("Distance cannot be null");

            if (duration == null)
                throw new DomainException("Duration cannot be null");

            if (distance.Value == 0)
                throw new DomainException("Distance cannot be zero");

            // Normaliza para km ou milhas
            Distance normalizedDistance;
            if (distance.Unit == MeasurementUnit.Meter)
            {
                normalizedDistance = distance.ConvertTo(MeasurementUnit.Kilometer);
            }
            else if (distance.Unit != MeasurementUnit.Kilometer && distance.Unit != MeasurementUnit.Mile)
            {
                normalizedDistance = distance.ConvertTo(MeasurementUnit.Kilometer);
            }
            else
            {
                normalizedDistance = distance;
            }

            var durationInSeconds = duration.ToTimeSpan().TotalSeconds;
            var timePerUnit = TimeSpan.FromSeconds(durationInSeconds / (double)normalizedDistance.Value);

            return new Pace(timePerUnit, normalizedDistance.Unit);
        }

        // Converte para o formato padrão de minutos:segundos por unidade
        public string ToDisplayString()
        {
            var totalMinutes = (int)TimePerUnit.TotalMinutes;
            var seconds = (int)TimePerUnit.Seconds;

            return $"{totalMinutes}:{seconds:D2}/{DistanceUnit}";
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TimePerUnit;
            yield return DistanceUnit;
        }
    }
}
