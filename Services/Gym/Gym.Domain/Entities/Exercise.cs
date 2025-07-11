﻿using Core.Domain.Entities;
using Gym.Domain.Enums;

namespace Gym.Domain.Entities
{
    public class Exercise : BaseEntity<int>
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public MuscleGroup MuscleGroup { get; private set; }
        public ExerciseType Type { get; private set; }
        public EquipmentType? EquipmentType { get; private set; }

        private Exercise() { }

        public Exercise(
            string name,
            string description,
            MuscleGroup muscleGroup,
            ExerciseType type,
            EquipmentType? equipmentType = null)
        {
            Validate(name);
            Validate(description);

            Name = name;
            Description = description;
            MuscleGroup = muscleGroup;
            Type = type;
            EquipmentType = equipmentType;
        }

        public void Update(
            string name,
            string description,
            MuscleGroup muscleGroup,
            ExerciseType type)
        {
            Validate(name);
            Validate(description);

            Name = name;
            Description = description;
            MuscleGroup = muscleGroup;
            Type = type;

            MarkAsUpdated();
        }

        public void SetEquipmentType(EquipmentType? equipmentType)
        {
            EquipmentType = equipmentType;
            MarkAsUpdated();
        }

        private void Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or empty.", nameof(value));
        }
    }
}