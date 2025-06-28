using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Gym.Domain.Enums;

namespace Gym.Domain.Entities
{
    public class ActivityVariation : BaseEntity<int>
    {
        public int ActivityId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public ActivityLevel Level { get; private set; }

        protected ActivityVariation() { }

        public ActivityVariation(int activityId, string name, string description, ActivityLevel level)
        {
            if (activityId <= 0)
                throw new DomainException("Activity ID cannot be negative");

            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Variation name cannot be empty");

            ActivityId = activityId;
            Name = name;
            Description = description ?? "";
            Level = level;
        }

        public void Update(string name, string description, ActivityLevel level)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Variation name cannot be empty");

            Name = name;
            Description = description ?? "";
            Level = level;
        }
    }
}
