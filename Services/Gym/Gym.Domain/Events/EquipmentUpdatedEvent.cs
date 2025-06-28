using Core.Domain.Events;

namespace Gym.Domain.Events
{
    public class EquipmentUpdatedEvent : DomainEvent
    {
        public int EquipmentId { get; }

        public EquipmentUpdatedEvent(int equipmentId)
        {
            EquipmentId = equipmentId;
        }
    }
}
