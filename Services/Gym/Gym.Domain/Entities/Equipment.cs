using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Gym.Domain.Enums;
using Gym.Domain.Events;

namespace Gym.Domain.Entities
{
    public class Equipment : BaseEntity<int>, IAggregateRoot
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public EquipmentCategory Category { get; private set; }
        public bool IsAvailable { get; private set; }
        public string ImageUrl { get; private set; }
        public string InstructionsUrl { get; private set; }
        public string Manufacturer { get; private set; }
        public string Model { get; private set; }
        public DateTime? PurchaseDate { get; private set; }

        protected Equipment()
        {
        }

        public Equipment(
            string name,
            string description,
            EquipmentCategory category,
            string manufacturer = null,
            string model = null,
            string imageUrl = null,
            string instructionsUrl = null)
        {
            ValidateName(name);

            Name = name;
            Description = description ?? "";
            Category = category;
            IsAvailable = true;
            ImageUrl = imageUrl;
            InstructionsUrl = instructionsUrl;
            Manufacturer = manufacturer;
            Model = model;

            AddDomainEvent(new EquipmentCreatedEvent(Id, name));
        }

        // Validações
        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Equipment name cannot be empty");

            if (name.Length > 100)
                throw new DomainException("Equipment name cannot exceed 100 characters");
        }

        // Métodos de negócio
        public void UpdateBasicInfo(string name, string description, EquipmentCategory category)
        {
            ValidateName(name);

            Name = name;
            Description = description ?? "";
            Category = category;

            AddDomainEvent(new EquipmentUpdatedEvent(Id));
        }

        public void UpdateUrls(string imageUrl, string instructionsUrl)
        {
            ImageUrl = imageUrl;
            InstructionsUrl = instructionsUrl;

            AddDomainEvent(new EquipmentUpdatedEvent(Id));
        }

        public void UpdateManufacturerInfo(string manufacturer, string model)
        {
            Manufacturer = manufacturer;
            Model = model;

            AddDomainEvent(new EquipmentUpdatedEvent(Id));
        }

        public void SetPurchaseDate(DateTime purchaseDate)
        {
            if (purchaseDate > DateTime.UtcNow)
                throw new DomainException("Purchase date cannot be in the future");

            PurchaseDate = purchaseDate;

            AddDomainEvent(new EquipmentUpdatedEvent(Id));
        }

        public void MarkAsAvailable()
        {
            IsAvailable = true;

            AddDomainEvent(new EquipmentUpdatedEvent(Id));
        }

        public void MarkAsUnavailable()
        {
            IsAvailable = false;

            AddDomainEvent(new EquipmentUpdatedEvent(Id));
        }
    }
}
