using BuildingBlocks.Results;
using FluentAssertions;
using Moq;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Application.Features.TaskLabels.Queries.GetById;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Enums;

namespace TaskManager.UnitTests.Application.Handlers.Queries.TaskLabels;

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
public class GetTaskLabelByIdQueryHandlerTests
{
    private readonly Mock<ITaskLabelService> _mockService;
    private readonly GetTaskLabelByIdQueryHandler _handler;

    public GetTaskLabelByIdQueryHandlerTests()
    {
        _mockService = new Mock<ITaskLabelService>();
        _handler = new GetTaskLabelByIdQueryHandler(_mockService.Object);
    }

    private static TaskLabelDTO CreateDto(int id = 1) => new(
        id, DateTime.UtcNow, null, $"Label {id}", LabelColor.Blue, 1);

    [Fact]
    public async Task Handle_WhenLabelExists_ShouldReturnSuccessWithLabel()
    {
        var dto = CreateDto(1);
        _mockService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<TaskLabelDTO?>(dto));

        var result = await _handler.Handle(new GetTaskLabelByIdQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.TaskLabel.Should().Be(dto);
    }

    [Fact]
    public async Task Handle_WhenLabelNotFound_ShouldReturnFailure()
    {
        var error = new Error("Label not found", ErrorType.NotFound);
        _mockService.Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<TaskLabelDTO?>(error));

        var result = await _handler.Handle(new GetTaskLabelByIdQuery(999), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
    }

    [Fact]
    public async Task Handle_ShouldCallServiceWithCorrectId()
    {
        var dto = CreateDto(7);
        _mockService.Setup(s => s.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<TaskLabelDTO?>(dto));

        await _handler.Handle(new GetTaskLabelByIdQuery(7), CancellationToken.None);

        _mockService.Verify(s => s.GetByIdAsync(7, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenLabelExists_ResultShouldContainCorrectLabelData()
    {
        var dto = CreateDto(4);
        _mockService.Setup(s => s.GetByIdAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<TaskLabelDTO?>(dto));

        var result = await _handler.Handle(new GetTaskLabelByIdQuery(4), CancellationToken.None);

        result.Value!.TaskLabel!.Id.Should().Be(4);
        result.Value.TaskLabel.Name.Should().Be("Label 4");
    }

    [Fact]
    public async Task Handle_ShouldPassCancellationToken()
    {
        var dto = CreateDto();
        var cts = new CancellationTokenSource();
        var token = cts.Token;
        _mockService.Setup(s => s.GetByIdAsync(1, token))
            .ReturnsAsync(Result.Success<TaskLabelDTO?>(dto));

        await _handler.Handle(new GetTaskLabelByIdQuery(1), token);

        _mockService.Verify(s => s.GetByIdAsync(1, token), Times.Once);
    }
}
