using BuildingBlocks.Results;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Application.Features.TaskItems.Commands.Delete;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Enums;

namespace TaskManager.UnitTests.Application.Handlers.Commands.TaskItems;

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
public class DeleteTaskItemCommandHandlerTests
{
    private readonly Mock<ITaskItemService> _mockService;
    private readonly Mock<IHttpContextAccessor> _mockHttpContext;
    private readonly DeleteTaskItemCommandHandler _handler;

    public DeleteTaskItemCommandHandlerTests()
    {
        _mockService = new Mock<ITaskItemService>();
        _mockHttpContext = new Mock<IHttpContextAccessor>();
        _handler = new DeleteTaskItemCommandHandler(_mockService.Object, _mockHttpContext.Object);
    }

    private static TaskItemDTO ValidTaskItemDto() => new(
        1, DateTime.UtcNow, null, "Task", "Description",
        Status.Pending, Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
        1, new List<TaskLabelDTO>());

    [Fact]
    public async Task Handle_WhenTaskExistsAndDeleteSucceeds_ShouldReturnSuccess()
    {
        _mockService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<TaskItemDTO?>(ValidTaskItemDto()));
        _mockService.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));

        var result = await _handler.Handle(new DeleteTaskItemCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenTaskNotFound_ShouldReturnNotFoundFailure()
    {
        var error = new Error("Task not found", ErrorType.NotFound);
        _mockService.Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<TaskItemDTO?>(error));

        var result = await _handler.Handle(new DeleteTaskItemCommand(999), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
        _mockService.Verify(s => s.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDeleteServiceFails_ShouldReturnFailure()
    {
        var error = new Error("Delete failed", ErrorType.Failure);
        _mockService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<TaskItemDTO?>(ValidTaskItemDto()));
        _mockService.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool>(error));

        var result = await _handler.Handle(new DeleteTaskItemCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
    }

    [Fact]
    public async Task Handle_ShouldCallGetByIdBeforeDelete()
    {
        var callOrder = new List<string>();
        _mockService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("GetById"))
            .ReturnsAsync(Result.Success<TaskItemDTO?>(ValidTaskItemDto()));
        _mockService.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("Delete"))
            .ReturnsAsync(Result.Success(true));

        await _handler.Handle(new DeleteTaskItemCommand(1), CancellationToken.None);

        callOrder.Should().ContainInOrder("GetById", "Delete");
    }
}
