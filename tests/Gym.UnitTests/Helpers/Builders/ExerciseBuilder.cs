using Gym.Domain.Entities;
using Gym.Domain.Enums;

namespace Gym.UnitTests.Helpers.Builders;

public class ExerciseBuilder
{
    private string _name = "Bench Press";
    private string _description = "Flat bench barbell press";
    private MuscleGroup _muscleGroup = MuscleGroup.Chest;
    private ExerciseType _type = ExerciseType.Strength;
    private EquipmentType? _equipmentType = EquipmentType.Barbell;

    public ExerciseBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ExerciseBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public ExerciseBuilder WithMuscleGroup(MuscleGroup muscleGroup)
    {
        _muscleGroup = muscleGroup;
        return this;
    }

    public ExerciseBuilder WithType(ExerciseType type)
    {
        _type = type;
        return this;
    }

    public ExerciseBuilder WithEquipmentType(EquipmentType? equipmentType)
    {
        _equipmentType = equipmentType;
        return this;
    }

    public Exercise Build()
    {
        return new Exercise(_name, _description, _muscleGroup, _type, _equipmentType);
    }

    public static ExerciseBuilder Default() => new();
}
