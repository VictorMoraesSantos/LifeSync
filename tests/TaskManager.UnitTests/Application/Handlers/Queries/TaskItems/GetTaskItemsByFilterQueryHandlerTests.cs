using BuildingBlocks.Results;
using FluentAssertions;
using Moq;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Application.Features.TaskItems.Queries.GetByFilter;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Enums;

namespace TaskManager.UnitTests.Application.Handlers.Queries.TaskItems;

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
public class GetTaskItemsByFilterQueryHandlerTests
{
    private readonly Mock<ITaskItemService> _mockService;
    private readonly GetTaskItemsByFilterQueryHandler _handler;

    public GetTaskItemsByFilterQueryHandlerTests()
    {
        _mockService = new Mock<ITaskItemService>();
        _handler = new GetTaskItemsByFilterQueryHandler(_mockService.Object);
    }

    private static TaskItemDTO CreateDto(int id = 1) => new(
        id, DateTime.UtcNow, null,
        $"Task {id}", $"Description {id}",
        Status.Pending, Priority.Medium,
        DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
        1, new List<TaskLabelDTO>());

    private static TaskItemFilterDTO DefaultFilter(int page = 1, int pageSize = 10) => new(
        null, null, null, null, null, null, null, null, null, null, null, null, page, pageSize);

    [Fact]
    public async Task Handle_WhenFilterReturnsItems_ShouldReturnSuccessWithResults()
    {
        var filter = DefaultFilter();
        var items = new List<TaskItemDTO> { CreateDto(1), CreateDto(2) };
        var pagination = new PaginationData(1, 10, 2, 1);

        _mockService.Setup(s => s.GetByFilterAsync(filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<(IEnumerable<TaskItemDTO>, PaginationData)>((items, pagination)));

        var result = await _handler.Handle(new GetTaskItemsByFilterQuery(filter), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.Pagination.Should().Be(pagination);
    }

    [Fact]
    public async Task Handle_WhenFilterReturnsEmpty_ShouldReturnEmptyList()
    {
        var filter = DefaultFilter();
        var pagination = new PaginationData(1, 10, 0, 0);

        _mockService.Setup(s => s.GetByFilterAsync(filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<(IEnumerable<TaskItemDTO>, PaginationData)>((new List<TaskItemDTO>(), pagination)));

        var result = await _handler.Handle(new GetTaskItemsByFilterQuery(filter), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenServiceFails_ShouldReturnFailure()
    {
        var filter = DefaultFilter();
        var error = new Error("Service error", ErrorType.Failure);

        _mockService.Setup(s => s.GetByFilterAsync(filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<(IEnumerable<TaskItemDTO>, PaginationData)>(error));

        var result = await _handler.Handle(new GetTaskItemsByFilterQuery(filter), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
    }

    [Fact]
    public async Task Handle_ShouldPassFilterToService()
    {
        var filter = new TaskItemFilterDTO(null, 5, "test", Status.Pending, Priority.High, null, null, null, null, null, "Title", false, 2, 20);

        _mockService.Setup(s => s.GetByFilterAsync(filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<(IEnumerable<TaskItemDTO>, PaginationData)>((new List<TaskItemDTO>(), new PaginationData(2, 20))));

        await _handler.Handle(new GetTaskItemsByFilterQuery(filter), CancellationToken.None);

        _mockService.Verify(s => s.GetByFilterAsync(filter, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPassCancellationToken()
    {
        var filter = DefaultFilter();
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        _mockService.Setup(s => s.GetByFilterAsync(filter, token))
            .ReturnsAsync(Result.Success<(IEnumerable<TaskItemDTO>, PaginationData)>((new List<TaskItemDTO>(), new PaginationData(1, 10))));

        await _handler.Handle(new GetTaskItemsByFilterQuery(filter), token);

        _mockService.Verify(s => s.GetByFilterAsync(filter, token), Times.Once);
    }
}
