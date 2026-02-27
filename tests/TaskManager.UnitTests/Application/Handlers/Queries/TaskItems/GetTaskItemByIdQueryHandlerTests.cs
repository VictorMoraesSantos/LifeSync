using BuildingBlocks.Results;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Application.Features.TaskItems.Queries.GetById;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Enums;

namespace TaskManager.UnitTests.Application.Handlers.Queries.TaskItems;

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
public class GetTaskItemByIdQueryHandlerTests
{
    private readonly Mock<ITaskItemService> _mockService;
    private readonly Mock<IHttpContextAccessor> _mockHttpContext;
    private readonly GetTaskItemByIdQueryHandler _handler;

    public GetTaskItemByIdQueryHandlerTests()
    {
        _mockService = new Mock<ITaskItemService>();
        _mockHttpContext = new Mock<IHttpContextAccessor>();
        _handler = new GetTaskItemByIdQueryHandler(_mockService.Object, _mockHttpContext.Object);
    }

    private static TaskItemDTO CreateDto(int id = 1) => new(
        id, DateTime.UtcNow, null,
        $"Task {id}", $"Description {id}",
        Status.Pending, Priority.Medium,
        DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
        1, new List<TaskLabelDTO>());

    [Fact]
    public async Task Handle_WhenTaskExists_ShouldReturnSuccessWithTaskItem()
    {
        var dto = CreateDto(1);
        _mockService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<TaskItemDTO?>(dto));

        var result = await _handler.Handle(new GetTaskItemByIdQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.TaskItem.Should().Be(dto);
    }

    [Fact]
    public async Task Handle_WhenTaskNotFound_ShouldReturnFailure()
    {
        var error = new Error("Task not found", ErrorType.NotFound);
        _mockService.Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<TaskItemDTO?>(error));

        var result = await _handler.Handle(new GetTaskItemByIdQuery(999), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
    }

    [Fact]
    public async Task Handle_ShouldCallServiceWithCorrectId()
    {
        var dto = CreateDto(5);
        _mockService.Setup(s => s.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<TaskItemDTO?>(dto));

        await _handler.Handle(new GetTaskItemByIdQuery(5), CancellationToken.None);

        _mockService.Verify(s => s.GetByIdAsync(5, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPassCancellationToken()
    {
        var dto = CreateDto();
        var cts = new CancellationTokenSource();
        var token = cts.Token;
        _mockService.Setup(s => s.GetByIdAsync(1, token))
            .ReturnsAsync(Result.Success<TaskItemDTO?>(dto));

        await _handler.Handle(new GetTaskItemByIdQuery(1), token);

        _mockService.Verify(s => s.GetByIdAsync(1, token), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTaskExists_ResultShouldContainCorrectTaskData()
    {
        var dto = CreateDto(3);
        _mockService.Setup(s => s.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<TaskItemDTO?>(dto));

        var result = await _handler.Handle(new GetTaskItemByIdQuery(3), CancellationToken.None);

        result.Value!.TaskItem.Id.Should().Be(3);
        result.Value.TaskItem.Title.Should().Be("Task 3");
    }
}
