using Gym.Domain.Entities;

namespace Gym.UnitTests.Helpers.Builders;

public class RoutineBuilder
{
    private string _name = "Push Day";
    private string _description = "Chest, shoulders and triceps workout";

    public RoutineBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public RoutineBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public Routine Build()
    {
        return new Routine(_name, _description);
    }

    public static RoutineBuilder Default() => new();
}
