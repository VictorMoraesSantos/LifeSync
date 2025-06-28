using Core.Domain.Exceptions;
using Core.Domain.ValueObjects;

namespace Gym.Domain.ValueObjects
{
    public class Intensity : ValueObject
    {
        public int Level { get; private set; } // 1-10
        public string Description { get; private set; }

        protected Intensity() { }

        public Intensity(int level, string description = null)
        {
            if (level < 1 || level > 10)
                throw new DomainException("Intensity level must be between 1 and 10");

            Level = level;
            Description = description ?? GetDefaultDescription(level);
        }

        private string GetDefaultDescription(int level)
        {
            return level switch
            {
                1 => "Very Light",
                2 => "Light",
                3 => "Moderate",
                4 => "Somewhat Hard",
                5 => "Hard",
                6 => "Harder",
                7 => "Very Hard",
                8 => "Very Very Hard",
                9 => "Extremely Hard",
                10 => "Maximum Effort",
                _ => "Unknown"
            };
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Level;
        }
    }
}
