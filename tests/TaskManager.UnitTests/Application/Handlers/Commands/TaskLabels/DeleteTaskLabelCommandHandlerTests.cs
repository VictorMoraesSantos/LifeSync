using BuildingBlocks.Results;
using FluentAssertions;
using Moq;
using TaskManager.Application.Features.TaskLabels.Commands.Delete;
using TaskManager.Application.Interfaces;

namespace TaskManager.UnitTests.Application.Handlers.Commands.TaskLabels;

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
public class DeleteTaskLabelCommandHandlerTests
{
    private readonly Mock<ITaskLabelService> _mockService;
    private readonly DeleteTaskLabelCommandHandler _handler;

    public DeleteTaskLabelCommandHandlerTests()
    {
        _mockService = new Mock<ITaskLabelService>();
        _handler = new DeleteTaskLabelCommandHandler(_mockService.Object);
    }

    [Fact]
    public async Task Handle_WhenLabelExistsAndDeleteSucceeds_ShouldReturnSuccess()
    {
        _mockService.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));

        var result = await _handler.Handle(new DeleteTaskLabelCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenLabelNotFound_ShouldReturnFailure()
    {
        var error = new Error("Label not found", ErrorType.NotFound);
        _mockService.Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool>(error));

        var result = await _handler.Handle(new DeleteTaskLabelCommand(999), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
    }

    [Fact]
    public async Task Handle_ShouldCallDeleteWithCorrectId()
    {
        _mockService.Setup(s => s.DeleteAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));

        await _handler.Handle(new DeleteTaskLabelCommand(42), CancellationToken.None);

        _mockService.Verify(s => s.DeleteAsync(42, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPassCancellationToken()
    {
        var cts = new CancellationTokenSource();
        var token = cts.Token;
        _mockService.Setup(s => s.DeleteAsync(1, token)).ReturnsAsync(Result.Success(true));

        await _handler.Handle(new DeleteTaskLabelCommand(1), token);

        _mockService.Verify(s => s.DeleteAsync(1, token), Times.Once);
    }
}
