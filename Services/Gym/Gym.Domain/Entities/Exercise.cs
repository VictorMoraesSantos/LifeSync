using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Gym.Domain.Enums;
using Gym.Domain.ValueObjects;

namespace Gym.Domain.Entities
{
    public class Exercise : BaseEntity<int>
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public MuscleGroup PrimaryMuscleGroup { get; private set; }
        public List<MuscleGroup> SecondaryMuscleGroups { get; private set; }
        public DifficultyLevel DifficultyLevel { get; private set; }
        public ExerciseType Type { get; private set; }
        public string InstructionsUrl { get; private set; }
        public string VideoUrl { get; private set; }
        private readonly List<Equipment> _requiredEquipment;
        public IReadOnlyCollection<Equipment> RequiredEquipment => _requiredEquipment.AsReadOnly();
        private readonly List<ExerciseExecution> _executions;
        public IReadOnlyCollection<ExerciseExecution> Executions => _executions.AsReadOnly();

        protected Exercise()
        {
            _requiredEquipment = new List<Equipment>();
            _executions = new List<ExerciseExecution>();
            SecondaryMuscleGroups = new List<MuscleGroup>();
        }

        public Exercise(
            string name,
            string description,
            MuscleGroup primaryMuscleGroup,
            DifficultyLevel difficultyLevel,
            ExerciseType type,
            string instructionsUrl = null,
            string videoUrl = null) : this()
        {
            UpdateName(name);
            UpdateDescription(description);
            PrimaryMuscleGroup = primaryMuscleGroup;
            DifficultyLevel = difficultyLevel;
            Type = type;
            InstructionsUrl = instructionsUrl;
            VideoUrl = videoUrl;
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Exercise name cannot be empty");

            Name = name;
        }

        public void UpdateDescription(string description)
        {
            Description = description ?? "";
        }

        public void UpdateDifficultyLevel(DifficultyLevel level)
        {
            DifficultyLevel = level;
        }

        public void UpdateExerciseType(ExerciseType type)
        {
            Type = type;
        }

        public void UpdateInstructionsUrl(string url)
        {
            InstructionsUrl = url;
        }

        public void UpdateVideoUrl(string url)
        {
            VideoUrl = url;
        }

        public void AddSecondaryMuscleGroup(MuscleGroup muscleGroup)
        {
            if (muscleGroup == PrimaryMuscleGroup)
                throw new DomainException("Secondary muscle group cannot be the same as primary muscle group");

            if (!SecondaryMuscleGroups.Contains(muscleGroup))
                SecondaryMuscleGroups.Add(muscleGroup);
        }

        public void RemoveSecondaryMuscleGroup(MuscleGroup muscleGroup)
        {
            SecondaryMuscleGroups.Remove(muscleGroup);
        }

        public void AddEquipment(Equipment equipment)
        {
            if (!_requiredEquipment.Contains(equipment))
                _requiredEquipment.Add(equipment);
        }

        public void RemoveEquipment(Equipment equipment)
        {
            _requiredEquipment.Remove(equipment);
        }

        public ExerciseExecution RecordExecution(
            SetRepetition setRepetition,
            DateTime executionDate,
            string notes = null,
            int? workoutId = null)
        {
            var execution = new ExerciseExecution(Id, setRepetition, executionDate, notes, workoutId);
            _executions.Add(execution);
            return execution;
        }
    }
}
