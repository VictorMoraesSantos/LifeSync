using Core.Domain.Exceptions;
using Core.Domain.ValueObjects;
using Gym.Domain.Enums;

namespace Gym.Domain.ValueObjects
{
    public class Equipment : ValueObject
    {
        public EquipmentType Type { get; private set; }
        public string Name { get; private set; }
        public bool IsRequired { get; private set; }

        protected Equipment() { } // For ORM

        public Equipment(EquipmentType type, string name, bool isRequired)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Equipment name cannot be empty");

            Type = type;
            Name = name;
            IsRequired = isRequired;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Type;
            yield return Name.ToLowerInvariant();
            yield return IsRequired;
        }
    }
}
