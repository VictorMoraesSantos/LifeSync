using BuildingBlocks.Results;
using FluentAssertions;
using Moq;
using TaskManager.Application.DTOs.TaskLabel.TaskLabel;
using TaskManager.Application.Features.TaskLabels.Commands.Create;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Enums;

namespace TaskManager.UnitTests.Application.Handlers.Commands.TaskLabels;

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
public class CreateTaskLabelCommandHandlerTests
{
    private readonly Mock<ITaskLabelService> _mockService;
    private readonly CreateTaskLabelCommandHandler _handler;

    public CreateTaskLabelCommandHandlerTests()
    {
        _mockService = new Mock<ITaskLabelService>();
        _handler = new CreateTaskLabelCommandHandler(_mockService.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var command = new CreateTaskLabelCommand("Work", LabelColor.Blue, 1);

        _mockService
            .Setup(s => s.CreateAsync(It.IsAny<CreateTaskLabelDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(42));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(42);

        _mockService.Verify(
            s => s.CreateAsync(
                It.Is<CreateTaskLabelDTO>(dto =>
                    dto.Name == "Work" &&
                    dto.LabelColor == LabelColor.Blue &&
                    dto.UserId == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenServiceReturnsFailure_ReturnsFailureResult()
    {
        // Arrange
        var command = new CreateTaskLabelCommand("Work", LabelColor.Blue, 1);
        var error = new Error("Service error", ErrorType.Failure);

        _mockService
            .Setup(s => s.CreateAsync(It.IsAny<CreateTaskLabelDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<int>(error));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
    }

    [Fact]
    public async Task Handle_CallsServiceWithCorrectDTO()
    {
        // Arrange
        var command = new CreateTaskLabelCommand("Personal", LabelColor.Green, 5);
        CreateTaskLabelDTO? capturedDto = null;

        _mockService
            .Setup(s => s.CreateAsync(It.IsAny<CreateTaskLabelDTO>(), It.IsAny<CancellationToken>()))
            .Callback<CreateTaskLabelDTO, CancellationToken>((dto, _) => capturedDto = dto)
            .ReturnsAsync(Result.Success(1));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedDto.Should().NotBeNull();
        capturedDto!.Name.Should().Be("Personal");
        capturedDto.LabelColor.Should().Be(LabelColor.Green);
        capturedDto.UserId.Should().Be(5);
    }

    [Fact]
    public async Task Handle_PassesCancellationTokenToService()
    {
        // Arrange
        var command = new CreateTaskLabelCommand("Work", LabelColor.Blue, 1);
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        _mockService
            .Setup(s => s.CreateAsync(It.IsAny<CreateTaskLabelDTO>(), token))
            .ReturnsAsync(Result.Success(1));

        // Act
        await _handler.Handle(command, token);

        // Assert
        _mockService.Verify(
            s => s.CreateAsync(It.IsAny<CreateTaskLabelDTO>(), token),
            Times.Once);
    }

    [Theory]
    [InlineData(LabelColor.Red)]
    [InlineData(LabelColor.Green)]
    [InlineData(LabelColor.Blue)]
    [InlineData(LabelColor.Yellow)]
    public async Task Handle_WithDifferentColors_PassesCorrectColorToService(LabelColor color)
    {
        // Arrange
        var command = new CreateTaskLabelCommand("Label", color, 1);

        _mockService
            .Setup(s => s.CreateAsync(It.IsAny<CreateTaskLabelDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(1));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockService.Verify(
            s => s.CreateAsync(
                It.Is<CreateTaskLabelDTO>(dto => dto.LabelColor == color),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithLongName_PassesFullNameToService()
    {
        // Arrange
        var longName = new string('A', 50);
        var command = new CreateTaskLabelCommand(longName, LabelColor.Purple, 1);

        _mockService
            .Setup(s => s.CreateAsync(It.IsAny<CreateTaskLabelDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(1));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockService.Verify(
            s => s.CreateAsync(
                It.Is<CreateTaskLabelDTO>(dto => dto.Name == longName),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
