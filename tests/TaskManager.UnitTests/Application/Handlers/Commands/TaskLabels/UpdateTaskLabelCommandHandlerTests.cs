using BuildingBlocks.Results;
using FluentAssertions;
using Moq;
using TaskManager.Application.DTOs.TaskLabel.TaskLabel;
using TaskManager.Application.Features.TaskLabels.Commands.Update;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Enums;

namespace TaskManager.UnitTests.Application.Handlers.Commands.TaskLabels;

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
public class UpdateTaskLabelCommandHandlerTests
{
    private readonly Mock<ITaskLabelService> _mockService;
    private readonly UpdateTaskLabelCommandHandler _handler;

    public UpdateTaskLabelCommandHandlerTests()
    {
        _mockService = new Mock<ITaskLabelService>();
        _handler = new UpdateTaskLabelCommandHandler(_mockService.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnSuccessResult()
    {
        var command = new UpdateTaskLabelCommand(1, "Updated Label", LabelColor.Red);
        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateTaskLabelDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.IsUpdated.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenServiceReturnsFailure_ShouldReturnFailure()
    {
        var command = new UpdateTaskLabelCommand(999, "Label", LabelColor.Blue);
        var error = new Error("Label not found", ErrorType.NotFound);
        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateTaskLabelDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool>(error));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
    }

    [Fact]
    public async Task Handle_ShouldMapCommandToDto_WithCorrectValues()
    {
        var command = new UpdateTaskLabelCommand(5, "Work Label", LabelColor.Green);
        UpdateTaskLabelDTO? capturedDto = null;

        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateTaskLabelDTO>(), It.IsAny<CancellationToken>()))
            .Callback<UpdateTaskLabelDTO, CancellationToken>((dto, _) => capturedDto = dto)
            .ReturnsAsync(Result.Success(true));

        await _handler.Handle(command, CancellationToken.None);

        capturedDto.Should().NotBeNull();
        capturedDto!.Id.Should().Be(5);
        capturedDto.Name.Should().Be("Work Label");
        capturedDto.LabelColor.Should().Be(LabelColor.Green);
    }

    [Fact]
    public async Task Handle_ShouldPassCancellationToken()
    {
        var command = new UpdateTaskLabelCommand(1, "Label", LabelColor.Blue);
        var cts = new CancellationTokenSource();
        var token = cts.Token;
        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateTaskLabelDTO>(), token))
            .ReturnsAsync(Result.Success(true));

        await _handler.Handle(command, token);

        _mockService.Verify(s => s.UpdateAsync(It.IsAny<UpdateTaskLabelDTO>(), token), Times.Once);
    }

    [Theory]
    [InlineData(LabelColor.Red)]
    [InlineData(LabelColor.Blue)]
    [InlineData(LabelColor.Purple)]
    [InlineData(LabelColor.Orange)]
    public async Task Handle_WithDifferentColors_ShouldPassCorrectColorToService(LabelColor color)
    {
        var command = new UpdateTaskLabelCommand(1, "Label", color);
        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateTaskLabelDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));

        await _handler.Handle(command, CancellationToken.None);

        _mockService.Verify(s => s.UpdateAsync(
            It.Is<UpdateTaskLabelDTO>(dto => dto.LabelColor == color),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
