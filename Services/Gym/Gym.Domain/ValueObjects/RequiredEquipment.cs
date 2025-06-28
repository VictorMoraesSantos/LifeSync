using Core.Domain.Exceptions;
using Core.Domain.ValueObjects;

namespace Gym.Domain.ValueObjects
{
    public class RequiredEquipment : ValueObject
    {
        public int EquipmentId { get; private set; }
        public string Name { get; private set; }
        public bool IsRequired { get; private set; }

        protected RequiredEquipment() { }

        public RequiredEquipment(int equipmentId, string name, bool isRequired)
        {
            if (equipmentId <= 0)
                throw new DomainException("Equipment ID cannot be negative");

            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Equipment name cannot be empty");

            EquipmentId = equipmentId;
            Name = name;
            IsRequired = isRequired;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return EquipmentId;
            yield return Name.ToLowerInvariant();
            yield return IsRequired;
        }
    }
}
