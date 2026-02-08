using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.UnitTests.Helpers.Builders;

public class TaskItemBuilder
{
    private string _title = "Default Title";
    private string _description = "Default Description";
    private Priority _priority = Priority.Medium;
    private DateOnly _dueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
    private int _userId = 1;
    private List<TaskLabel>? _labels = null;

    public TaskItemBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public TaskItemBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public TaskItemBuilder WithPriority(Priority priority)
    {
        _priority = priority;
        return this;
    }

    public TaskItemBuilder WithDueDate(DateOnly dueDate)
    {
        _dueDate = dueDate;
        return this;
    }

    public TaskItemBuilder WithUserId(int userId)
    {
        _userId = userId;
        return this;
    }

    public TaskItemBuilder WithLabels(params TaskLabel[] labels)
    {
        _labels = labels.ToList();
        return this;
    }

    public TaskItem Build()
    {
        return new TaskItem(_title, _description, _priority, _dueDate, _userId, _labels);
    }

    public static TaskItemBuilder Default() => new();
}
