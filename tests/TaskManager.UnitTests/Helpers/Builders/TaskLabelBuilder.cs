using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.UnitTests.Helpers.Builders;

public class TaskLabelBuilder
{
    private string _name = "Default Label";
    private LabelColor _color = LabelColor.Blue;
    private int _userId = 1;

    public TaskLabelBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public TaskLabelBuilder WithColor(LabelColor color)
    {
        _color = color;
        return this;
    }

    public TaskLabelBuilder WithUserId(int userId)
    {
        _userId = userId;
        return this;
    }

    public TaskLabel Build()
    {
        return new TaskLabel(_name, _color, _userId);
    }

    public static TaskLabelBuilder Default() => new();
}
