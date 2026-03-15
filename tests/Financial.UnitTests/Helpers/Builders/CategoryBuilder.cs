using Financial.Domain.Entities;

namespace Financial.UnitTests.Helpers.Builders;

public class CategoryBuilder
{
    private int _userId = 1;
    private string _name = "Test Category";
    private string? _description = "Test description";

    public CategoryBuilder WithUserId(int userId)
    {
        _userId = userId;
        return this;
    }

    public CategoryBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CategoryBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public Category Build()
    {
        return new Category(_userId, _name, _description);
    }

    public static CategoryBuilder Default() => new();
}
