using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Gym.Domain.Enums;
using Gym.Domain.Events;
using Gym.Domain.ValueObjects;

namespace Gym.Domain.Entities
{
    public class TrainingActivity : BaseEntity<int>, IAggregateRoot
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public TrainingType Type { get; private set; }
        public ActivityLevel Level { get; private set; }
        public string InstructionsUrl { get; private set; }
        public string VideoUrl { get; private set; }
        public int CaloriesBurnedPerMinute { get; private set; }

        // Coleções
        private readonly List<ActivityVariation> _variations;
        public IReadOnlyCollection<ActivityVariation> Variations => _variations.AsReadOnly();

        private readonly List<RequiredEquipment> _equipment;
        public IReadOnlyCollection<RequiredEquipment> Equipment => _equipment.AsReadOnly();

        // Especificações para tipos específicos de atividades
        public TargetMuscles TargetMuscles { get; private set; }
        public RunningDetails RunningDetails { get; private set; }

        protected TrainingActivity()
        {
            _variations = new List<ActivityVariation>();
            _equipment = new List<RequiredEquipment>();
        }

        // Construtor para atividades de musculação
        public TrainingActivity(
            string name,
            string description,
            TrainingType type,
            ActivityLevel level,
            TargetMuscles targetMuscles,
            int caloriesBurnedPerMinute = 0,
            string instructionsUrl = null,
            string videoUrl = null)
        {
            ValidateName(name);
            ValidateDescription(description);
            ValidateType(type, targetMuscles, null);

            Name = name;
            Description = description;
            Type = type;
            Level = level;
            CaloriesBurnedPerMinute = caloriesBurnedPerMinute;
            InstructionsUrl = instructionsUrl;
            VideoUrl = videoUrl;
            TargetMuscles = targetMuscles;
            RunningDetails = null;

            _variations = new List<ActivityVariation>();
            _equipment = new List<RequiredEquipment>();

            AddDomainEvent(new TrainingActivityCreatedEvent(Id, name));
        }

        // Construtor para atividades de corrida
        public TrainingActivity(
            string name,
            string description,
            TrainingType type,
            ActivityLevel level,
            RunningDetails runningDetails,
            int caloriesBurnedPerMinute = 0,
            string instructionsUrl = null,
            string videoUrl = null)
        {
            ValidateName(name);
            ValidateDescription(description);
            ValidateType(type, null, runningDetails);

            Name = name;
            Description = description;
            Type = type;
            Level = level;
            CaloriesBurnedPerMinute = caloriesBurnedPerMinute;
            InstructionsUrl = instructionsUrl;
            VideoUrl = videoUrl;
            TargetMuscles = null;
            RunningDetails = runningDetails;

            _variations = new List<ActivityVariation>();
            _equipment = new List<RequiredEquipment>();

            AddDomainEvent(new TrainingActivityCreatedEvent(Id, name));
        }

        // Validações
        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Activity name cannot be empty");

            if (name.Length > 100)
                throw new DomainException("Activity name cannot exceed 100 characters");
        }

        private void ValidateDescription(string description)
        {
            if (description?.Length > 500)
                throw new DomainException("Activity description cannot exceed 500 characters");
        }

        private void ValidateType(TrainingType type, TargetMuscles targetMuscles, RunningDetails runningDetails)
        {
            bool isStrengthType = type == TrainingType.Strength ||
                                 type == TrainingType.Hypertrophy ||
                                 type == TrainingType.Power ||
                                 type == TrainingType.Endurance;

            bool isCardioType = type == TrainingType.Cardio ||
                               type == TrainingType.HIIT;

            if (isStrengthType && targetMuscles == null)
                throw new DomainException("Strength activities must specify target muscles");

            if (isCardioType && runningDetails == null && type != TrainingType.HIIT)
                throw new DomainException("Cardio activities must specify running details");
        }

        // Métodos de negócio
        public void UpdateBasicInfo(string name, string description, ActivityLevel level)
        {
            ValidateName(name);
            ValidateDescription(description);

            Name = name;
            Description = description;
            Level = level;

            AddDomainEvent(new TrainingActivityUpdatedEvent(Id));
        }

        public void UpdateUrls(string instructionsUrl, string videoUrl)
        {
            InstructionsUrl = instructionsUrl;
            VideoUrl = videoUrl;

            AddDomainEvent(new TrainingActivityUpdatedEvent(Id));
        }

        public void UpdateCaloriesBurned(int caloriesBurnedPerMinute)
        {
            if (caloriesBurnedPerMinute < 0)
                throw new DomainException("Calories burned cannot be negative");

            CaloriesBurnedPerMinute = caloriesBurnedPerMinute;

            AddDomainEvent(new TrainingActivityUpdatedEvent(Id));
        }

        public void AddVariation(string name, string description, ActivityLevel level)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Variation name cannot be empty");

            var variation = new ActivityVariation(Id, name, description, level);
            _variations.Add(variation);

            AddDomainEvent(new TrainingActivityUpdatedEvent(Id));
        }

        public void RemoveVariation(int variationId)
        {
            var variation = _variations.FirstOrDefault(v => v.Id == variationId);
            if (variation != null)
            {
                _variations.Remove(variation);
                AddDomainEvent(new TrainingActivityUpdatedEvent(Id));
            }
        }

        public void AddEquipment(int equipmentId, string equipmentName, bool isRequired)
        {
            var equipment = new RequiredEquipment(equipmentId, equipmentName, isRequired);

            if (!_equipment.Contains(equipment))
            {
                _equipment.Add(equipment);
                AddDomainEvent(new TrainingActivityUpdatedEvent(Id));
            }
        }

        public void RemoveEquipment(int equipmentId)
        {
            var equipment = _equipment.FirstOrDefault(e => e.EquipmentId == equipmentId);
            if (equipment != null)
            {
                _equipment.Remove(equipment);
                AddDomainEvent(new TrainingActivityUpdatedEvent(Id));
            }
        }

        public void UpdateTargetMuscles(TargetMuscles targetMuscles)
        {
            if (RunningDetails != null)
                throw new DomainException("Cannot add target muscles to a running activity");

            TargetMuscles = targetMuscles ?? throw new DomainException("Target muscles cannot be null");

            AddDomainEvent(new TrainingActivityUpdatedEvent(Id));
        }

        public void UpdateRunningDetails(RunningDetails runningDetails)
        {
            if (TargetMuscles != null)
                throw new DomainException("Cannot add running details to a strength activity");

            RunningDetails = runningDetails ?? throw new DomainException("Running details cannot be null");

            AddDomainEvent(new TrainingActivityUpdatedEvent(Id));
        }
    }
}
