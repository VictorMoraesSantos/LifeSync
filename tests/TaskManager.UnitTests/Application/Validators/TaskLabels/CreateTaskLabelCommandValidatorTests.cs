using FluentAssertions;
using TaskManager.Application.Features.TaskLabels.Commands.Create;
using TaskManager.Domain.Enums;

namespace TaskManager.UnitTests.Application.Validators.TaskLabels;

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
public class CreateTaskLabelCommandValidatorTests
{
    private readonly CreateTaskLabelCommandValidator _validator;

    public CreateTaskLabelCommandValidatorTests()
    {
        _validator = new CreateTaskLabelCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldBeValid()
    {
        // Arrange
        var command = new CreateTaskLabelCommand("Work", LabelColor.Blue, 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Validate_WithEmptyName_ShouldBeInvalid(string name)
    {
        // Arrange
        var command = new CreateTaskLabelCommand(name, LabelColor.Blue, 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        result.Errors.Where(e => e.PropertyName == "Name").Should().Contain(e =>
            e.ErrorMessage == "O nome da etiqueta é obrigatório.");
    }

    [Theory]
    [InlineData("A")]
    [InlineData("B")]
    public void Validate_WithShortName_ShouldBeInvalid(string name)
    {
        // Arrange
        var command = new CreateTaskLabelCommand(name, LabelColor.Blue, 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Name");
        result.Errors.First().ErrorMessage.Should().Be("O nome da etiqueta deve ter no mínimo 2 caracteres.");
    }

    [Fact]
    public void Validate_WithLongName_ShouldBeInvalid()
    {
        // Arrange
        var longName = new string('A', 51);
        var command = new CreateTaskLabelCommand(longName, LabelColor.Blue, 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Name");
        result.Errors.First().ErrorMessage.Should().Be("O nome da etiqueta deve ter no máximo 50 caracteres.");
    }

    [Fact]
    public void Validate_WithMinimumValidName_ShouldBeValid()
    {
        // Arrange
        var command = new CreateTaskLabelCommand("AB", LabelColor.Blue, 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithMaximumValidName_ShouldBeValid()
    {
        // Arrange
        var maxName = new string('A', 50);
        var command = new CreateTaskLabelCommand(maxName, LabelColor.Blue, 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithInvalidColor_ShouldBeInvalid()
    {
        // Arrange
        var command = new CreateTaskLabelCommand("Valid Name", (LabelColor)999, 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "LabelColor");
        result.Errors.First().ErrorMessage.Should().Be("A cor da etiqueta informada é inválida.");
    }

    [Theory]
    [InlineData(LabelColor.Red)]
    [InlineData(LabelColor.Green)]
    [InlineData(LabelColor.Blue)]
    [InlineData(LabelColor.Yellow)]
    [InlineData(LabelColor.Purple)]
    [InlineData(LabelColor.Orange)]
    [InlineData(LabelColor.Pink)]
    [InlineData(LabelColor.Brown)]
    [InlineData(LabelColor.Gray)]
    public void Validate_WithValidColors_ShouldBeValid(LabelColor color)
    {
        // Arrange
        var command = new CreateTaskLabelCommand("Work", color, 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
