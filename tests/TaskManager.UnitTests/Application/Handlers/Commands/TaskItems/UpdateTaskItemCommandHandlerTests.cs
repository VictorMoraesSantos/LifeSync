using BuildingBlocks.Results;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Application.Features.TaskItems.Commands.Update;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Enums;

namespace TaskManager.UnitTests.Application.Handlers.Commands.TaskItems;

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
public class UpdateTaskItemCommandHandlerTests
{
    private readonly Mock<ITaskItemService> _mockService;
    private readonly Mock<IValidator<UpdateTaskItemCommand>> _mockValidator;
    private readonly Mock<IHttpContextAccessor> _mockHttpContext;
    private readonly UpdateTaskItemCommandHandler _handler;

    public UpdateTaskItemCommandHandlerTests()
    {
        _mockService = new Mock<ITaskItemService>();
        _mockValidator = new Mock<IValidator<UpdateTaskItemCommand>>();
        _mockHttpContext = new Mock<IHttpContextAccessor>();
        _handler = new UpdateTaskItemCommandHandler(_mockService.Object, _mockValidator.Object, _mockHttpContext.Object);
    }

    private static TaskItemDTO ValidTaskItemDto() => new(
        1, DateTime.UtcNow, null, "Existing Task", "Existing Description",
        Status.Pending, Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
        1, new List<TaskLabelDTO>());

    private static UpdateTaskItemCommand ValidCommand() => new(
        1, "Updated Task", "Updated Description", Status.InProgress,
        Priority.High, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)));

    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnSuccessResult()
    {
        var command = ValidCommand();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<TaskItemDTO?>(ValidTaskItemDto()));
        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateTaskItemDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.IsUpdated.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailureWithoutCallingService()
    {
        var command = ValidCommand();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Title", "Title is required") }));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _mockService.Verify(s => s.UpdateAsync(It.IsAny<UpdateTaskItemDTO>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenTaskNotFound_ShouldReturnNotFoundFailure()
    {
        var command = ValidCommand();
        var error = new Error("Task not found", ErrorType.NotFound);
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<TaskItemDTO?>(error));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
        _mockService.Verify(s => s.UpdateAsync(It.IsAny<UpdateTaskItemDTO>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUpdateServiceFails_ShouldReturnFailure()
    {
        var command = ValidCommand();
        var error = new Error("Update failed", ErrorType.Failure);
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<TaskItemDTO?>(ValidTaskItemDto()));
        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateTaskItemDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool>(error));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
    }

    [Fact]
    public async Task Handle_ShouldMapCommandToUpdateDto_WithCorrectId()
    {
        var command = ValidCommand();
        UpdateTaskItemDTO? capturedDto = null;

        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<TaskItemDTO?>(ValidTaskItemDto()));
        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateTaskItemDTO>(), It.IsAny<CancellationToken>()))
            .Callback<UpdateTaskItemDTO, CancellationToken>((dto, _) => capturedDto = dto)
            .ReturnsAsync(Result.Success(true));

        await _handler.Handle(command, CancellationToken.None);

        capturedDto.Should().NotBeNull();
        capturedDto!.Id.Should().Be(command.Id);
        capturedDto.Title.Should().Be(command.Title);
        capturedDto.Status.Should().Be(command.Status);
        capturedDto.Priority.Should().Be(command.Priority);
    }
}
