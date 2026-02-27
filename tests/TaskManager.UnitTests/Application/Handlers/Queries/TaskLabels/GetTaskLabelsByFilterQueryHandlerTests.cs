using BuildingBlocks.Results;
using FluentAssertions;
using Moq;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Application.Features.TaskLabels.Queries.GetByFilter;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Enums;

namespace TaskManager.UnitTests.Application.Handlers.Queries.TaskLabels;

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
public class GetTaskLabelsByFilterQueryHandlerTests
{
    private readonly Mock<ITaskLabelService> _mockService;
    private readonly GetTaskLabelsByFilterQueryHandler _handler;

    public GetTaskLabelsByFilterQueryHandlerTests()
    {
        _mockService = new Mock<ITaskLabelService>();
        _handler = new GetTaskLabelsByFilterQueryHandler(_mockService.Object);
    }

    private static TaskLabelDTO CreateDto(int id = 1) => new(
        id, DateTime.UtcNow, null, $"Label {id}", LabelColor.Blue, 1);

    private static TaskLabelFilterDTO DefaultFilter(int page = 1, int pageSize = 10) => new(
        null, null, null, null, null, null, null, null, null, null, page, pageSize);

    [Fact]
    public async Task Handle_WhenFilterReturnsItems_ShouldReturnSuccessWithResults()
    {
        var filter = DefaultFilter();
        var items = new List<TaskLabelDTO> { CreateDto(1), CreateDto(2) };
        var pagination = new PaginationData(1, 10, 2, 1);

        _mockService.Setup(s => s.GetByFilterAsync(filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<(IEnumerable<TaskLabelDTO>, PaginationData)>((items, pagination)));

        var result = await _handler.Handle(new GetTaskLabelsByFilterQuery(filter), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TaskLabels.Should().HaveCount(2);
        result.Value.Pagination.Should().Be(pagination);
    }

    [Fact]
    public async Task Handle_WhenFilterReturnsEmpty_ShouldReturnEmptyList()
    {
        var filter = DefaultFilter();
        var pagination = new PaginationData(1, 10, 0, 0);

        _mockService.Setup(s => s.GetByFilterAsync(filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<(IEnumerable<TaskLabelDTO>, PaginationData)>((new List<TaskLabelDTO>(), pagination)));

        var result = await _handler.Handle(new GetTaskLabelsByFilterQuery(filter), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TaskLabels.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenServiceFails_ShouldReturnFailure()
    {
        var filter = DefaultFilter();
        var error = new Error("Filter error", ErrorType.Failure);

        _mockService.Setup(s => s.GetByFilterAsync(filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<(IEnumerable<TaskLabelDTO>, PaginationData)>(error));

        var result = await _handler.Handle(new GetTaskLabelsByFilterQuery(filter), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
    }

    [Fact]
    public async Task Handle_ShouldPassFilterToService()
    {
        var filter = new TaskLabelFilterDTO(null, 3, null, "work", LabelColor.Blue, null, null, null, "Name", false, 1, 5);

        _mockService.Setup(s => s.GetByFilterAsync(filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<(IEnumerable<TaskLabelDTO>, PaginationData)>((new List<TaskLabelDTO>(), new PaginationData(1, 5))));

        await _handler.Handle(new GetTaskLabelsByFilterQuery(filter), CancellationToken.None);

        _mockService.Verify(s => s.GetByFilterAsync(filter, It.IsAny<CancellationToken>()), Times.Once);
    }
}
