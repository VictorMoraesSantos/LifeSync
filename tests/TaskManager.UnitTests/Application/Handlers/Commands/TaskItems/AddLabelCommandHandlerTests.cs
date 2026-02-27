using BuildingBlocks.Results;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Application.Features.TaskItems.Commands.AddLabel;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Enums;

namespace TaskManager.UnitTests.Application.Handlers.Commands.TaskItems;

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
public class AddLabelCommandHandlerTests
{
    private readonly Mock<ITaskItemService> _mockService;
    private readonly Mock<IValidator<AddLabelCommand>> _mockValidator;
    private readonly Mock<IHttpContextAccessor> _mockHttpContext;
    private readonly AddLabelCommandHandler _handler;

    public AddLabelCommandHandlerTests()
    {
        _mockService = new Mock<ITaskItemService>();
        _mockValidator = new Mock<IValidator<AddLabelCommand>>();
        _mockHttpContext = new Mock<IHttpContextAccessor>();
        _handler = new AddLabelCommandHandler(_mockService.Object, _mockValidator.Object, _mockHttpContext.Object);
    }

    private static TaskItemDTO ValidTaskItemDto() => new(
        1, DateTime.UtcNow, null, "Task", "Description",
        Status.Pending, Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
        1, new List<TaskLabelDTO>());

    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnSuccess()
    {
        var command = new AddLabelCommand(1, new List<int> { 10, 11 });
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<TaskItemDTO?>(ValidTaskItemDto()));
        _mockService.Setup(s => s.AddLabelAsync(It.IsAny<UpdateLabelsDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailureWithoutCallingService()
    {
        var command = new AddLabelCommand(1, new List<int>());
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("TaskLabelsId", "Labels list cannot be empty") }));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _mockService.Verify(s => s.AddLabelAsync(It.IsAny<UpdateLabelsDTO>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenTaskNotFound_ShouldReturnFailure()
    {
        var command = new AddLabelCommand(999, new List<int> { 1 });
        var error = new Error("Task not found", ErrorType.NotFound);
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<TaskItemDTO?>(error));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
        _mockService.Verify(s => s.AddLabelAsync(It.IsAny<UpdateLabelsDTO>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectDtoToAddLabelAsync()
    {
        var command = new AddLabelCommand(5, new List<int> { 10, 20 });
        UpdateLabelsDTO? capturedDto = null;

        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<TaskItemDTO?>(ValidTaskItemDto()));
        _mockService.Setup(s => s.AddLabelAsync(It.IsAny<UpdateLabelsDTO>(), It.IsAny<CancellationToken>()))
            .Callback<UpdateLabelsDTO, CancellationToken>((dto, _) => capturedDto = dto)
            .ReturnsAsync(Result.Success(true));

        await _handler.Handle(command, CancellationToken.None);

        capturedDto.Should().NotBeNull();
        capturedDto!.TaskItemId.Should().Be(5);
        capturedDto.TaskLabelsId.Should().Contain(new[] { 10, 20 });
    }
}
