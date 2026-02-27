using BuildingBlocks.Results;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.Features.TaskItems.Commands.Create;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Enums;

namespace TaskManager.UnitTests.Application.Handlers.Commands.TaskItems;

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
public class CreateTaskItemCommandHandlerTests
{
    private readonly Mock<ITaskItemService> _mockService;
    private readonly Mock<IValidator<CreateTaskItemCommand>> _mockValidator;
    private readonly Mock<IHttpContextAccessor> _mockHttpContext;
    private readonly CreateTaskItemCommandHandler _handler;

    public CreateTaskItemCommandHandlerTests()
    {
        _mockService = new Mock<ITaskItemService>();
        _mockValidator = new Mock<IValidator<CreateTaskItemCommand>>();
        _mockHttpContext = new Mock<IHttpContextAccessor>();
        _handler = new CreateTaskItemCommandHandler(_mockService.Object, _mockValidator.Object, _mockHttpContext.Object);
    }

    private static CreateTaskItemCommand ValidCommand() => new(
        "Test Task",
        "Test Description",
        Priority.Medium,
        DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
        1,
        null);

    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnSuccessWithId()
    {
        var command = ValidCommand();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.CreateAsync(It.IsAny<CreateTaskItemDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(42));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(42);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailureWithValidationError()
    {
        var command = ValidCommand();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Title", "Title is required") }));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Description.Should().Contain("Title is required");
        _mockService.Verify(s => s.CreateAsync(It.IsAny<CreateTaskItemDTO>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenServiceFails_ShouldReturnFailure()
    {
        var command = ValidCommand();
        var error = new Error("Service error", ErrorType.Failure);
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.CreateAsync(It.IsAny<CreateTaskItemDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<int>(error));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
    }

    [Fact]
    public async Task Handle_ShouldMapCommandToDto_WithCorrectValues()
    {
        var command = ValidCommand();
        CreateTaskItemDTO? capturedDto = null;

        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.CreateAsync(It.IsAny<CreateTaskItemDTO>(), It.IsAny<CancellationToken>()))
            .Callback<CreateTaskItemDTO, CancellationToken>((dto, _) => capturedDto = dto)
            .ReturnsAsync(Result.Success(1));

        await _handler.Handle(command, CancellationToken.None);

        capturedDto.Should().NotBeNull();
        capturedDto!.Title.Should().Be(command.Title);
        capturedDto.Description.Should().Be(command.Description);
        capturedDto.Priority.Should().Be(command.Priority);
        capturedDto.DueDate.Should().Be(command.DueDate);
        capturedDto.UserId.Should().Be(command.UserId);
    }

    [Fact]
    public async Task Handle_ShouldPassCancellationTokenToService()
    {
        var command = ValidCommand();
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        _mockValidator.Setup(v => v.ValidateAsync(command, token)).ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.CreateAsync(It.IsAny<CreateTaskItemDTO>(), token)).ReturnsAsync(Result.Success(1));

        await _handler.Handle(command, token);

        _mockService.Verify(s => s.CreateAsync(It.IsAny<CreateTaskItemDTO>(), token), Times.Once);
    }

    [Fact]
    public async Task Handle_WithMultipleValidationErrors_ShouldReturnAllErrors()
    {
        var command = ValidCommand();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure("Title", "Title is required"),
                new ValidationFailure("Description", "Description is required")
            }));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Description.Should().Contain("Title is required");
        result.Error.Description.Should().Contain("Description is required");
    }
}
