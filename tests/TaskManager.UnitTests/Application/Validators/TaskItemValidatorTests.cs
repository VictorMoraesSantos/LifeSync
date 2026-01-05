using TaskManager.Application.Features.TaskItems.Commands.Create;
using TaskManager.Domain.Enums;

namespace TaskManager.UnitTests.Application.Validators
{
    public class TaskItemValidatorTests
    {
        [Fact]
        public void Validate_WithValidCommand_ShouldBeValid()
        {
            //Arrange
            var command = new CreateTaskItemCommand("valid title", "valid description", Priority.Medium, DateOnly.FromDateTime(DateTime.Now.AddDays(1)), 1);
            var validator = new CreateTaskItemCommandValidator();

            //Act
            var result = validator.Validate(command);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void Validate_WithEmptyTitle_ShouldBeInvalid(string title)
        {
            //Arrange
            var command = new CreateTaskItemCommand(title, "desc", (Priority)100, DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), 1);
            var validator = new CreateTaskItemCommandValidator();

            //Act
            var result = validator.Validate(command);

            //Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "O título é obrigatório.");
        }

        [Theory]
        [InlineData("a")]
        [InlineData("ab")]
        public void Validade_WithShortTitle_ShouldBeInvalid(string title)
        {
            //Arrange
            var command = new CreateTaskItemCommand(title, "desc", Priority.Low, DateOnly.FromDateTime(DateTime.Now.AddDays(1)), 1);
            var validator = new CreateTaskItemCommandValidator();

            //Act
            var result = validator.Validate(command);

            //Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "O título deve ter no mínimo 3 caracteres.");
        }

        [Fact]
        public void Validate_WithLongTitle_ShouldBeInvalid()
        {
            //Arrange
            var longTitle = new string('a', 101);
            var command = new CreateTaskItemCommand(longTitle, "desc", Priority.Low, DateOnly.FromDateTime(DateTime.Now.AddDays(1)), 1);
            var validator = new CreateTaskItemCommandValidator();

            //Act
            var result = validator.Validate(command);

            //Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "O título deve ter no máximo 100 caracteres.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void Validate_WithEmptyDescription_ShouldBeInvalid(string description)
        {
            //Arrange
            var command = new CreateTaskItemCommand("valid title", description, Priority.Medium, DateOnly.FromDateTime(DateTime.Now.AddDays(1)), 1);
            var validator = new CreateTaskItemCommandValidator();

            //Act
            var result = validator.Validate(command);

            //Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "A descrição é obrigatória.");
        }

        [Theory]
        [InlineData("a")]
        [InlineData("ab")]
        [InlineData("abc")]
        [InlineData("abcd")]
        public void Validate_WithShortDescription_ShouldBeInvalid(string description)
        {
            //Arrange
            var command = new CreateTaskItemCommand("valid title", description, Priority.Low, DateOnly.FromDateTime(DateTime.Now.AddDays(1)), 1);
            var validator = new CreateTaskItemCommandValidator();

            //Act
            var result = validator.Validate(command);

            //Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "A descrição deve ter no mínimo 5 caracteres.");
        }

        [Fact]
        public void Validate_WithLongDescription_ShouldBeInvalid()
        {
            //Arrange
            var longDescription = new string('a', 501);
            var command = new CreateTaskItemCommand("valid title", longDescription, Priority.Low, DateOnly.FromDateTime(DateTime.Now.AddDays(1)), 1);
            var validator = new CreateTaskItemCommandValidator();

            //Act
            var result = validator.Validate(command);

            //Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "A descrição deve ter no máximo 500 caracteres.");
        }

        [Fact]
        public void Validate_WithInvalidPriority_ShouldBeInvalid()
        {
            //Arrange
            var command = new CreateTaskItemCommand("valid title", "valid description", (Priority)100, DateOnly.FromDateTime(DateTime.Now.AddDays(1)), 1);
            var validator = new CreateTaskItemCommandValidator();

            //Act
            var result = validator.Validate(command);

            //Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "A prioridade informada é inválida.");
        }

        [Fact]
        public void Validate_WithPastDueDate_ShouldBeInvalid()
        {
            //Arrange
            var pastDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
            var command = new CreateTaskItemCommand("valid title", "valid description", Priority.Medium, pastDueDate, 1);
            var validator = new CreateTaskItemCommandValidator();

            //Act
            var result = validator.Validate(command);

            //Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "A data de vencimento não pode ser anterior à data atual.");
        }
    }
}
