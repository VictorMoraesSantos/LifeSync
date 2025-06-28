using Core.Domain.Events;

namespace Gym.Domain.Events
{
    public class EquipmentCreatedEvent : DomainEvent
    {
        public int EquipmentId { get; }
        public string EquipmentName { get; }

        public EquipmentCreatedEvent(int equipmentId, string equipmentName)
        {
            EquipmentId = equipmentId;
            EquipmentName = equipmentName;
        }
    }
}
