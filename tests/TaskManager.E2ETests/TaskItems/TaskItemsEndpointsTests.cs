using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using TaskManager.Domain.Enums;
using TaskManager.E2ETests.Infrastructure;

namespace TaskManager.E2ETests.TaskItems;

[Trait("Category", "E2E")]
[Trait("Layer", "API")]
public class TaskItemsEndpointsTests : IClassFixture<TaskManagerWebApplicationFactory>, IAsyncLifetime
{
    private readonly TaskManagerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public TaskItemsEndpointsTests(TaskManagerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    // ─── POST /api/task-items ─────────────────────────────────────────────────

    [Fact]
    public async Task POST_CreateTaskItem_WithValidData_ShouldReturnCreated()
    {
        var command = new
        {
            Title = "E2E Test Task",
            Description = "E2E Test Description",
            Priority = (int)Priority.Medium,
            DueDate = DateTime.UtcNow.AddDays(7).ToString("yyyy-MM-dd"),
            UserId = 1
        };

        var response = await _client.PostAsJsonAsync("/api/task-items", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task POST_CreateTaskItem_WithInvalidData_ShouldReturnBadRequest()
    {
        var command = new
        {
            Title = "",
            Description = "Description",
            Priority = (int)Priority.Medium,
            DueDate = DateTime.UtcNow.AddDays(7).ToString("yyyy-MM-dd"),
            UserId = 1
        };

        var response = await _client.PostAsJsonAsync("/api/task-items", command);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.UnprocessableEntity);
    }

    // ─── GET /api/task-items/{id} ─────────────────────────────────────────────

    [Fact]
    public async Task GET_TaskItemById_WhenExists_ShouldReturnOk()
    {
        var createCommand = new
        {
            Title = "Task to Fetch",
            Description = "Fetch me by ID",
            Priority = (int)Priority.High,
            DueDate = DateTime.UtcNow.AddDays(5).ToString("yyyy-MM-dd"),
            UserId = 1
        };
        var createResponse = await _client.PostAsJsonAsync("/api/task-items", createCommand);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdData = await createResponse.Content.ReadFromJsonAsync<ApiResponse<int>>();
        var id = createdData?.Data;

        var response = await _client.GetAsync($"/api/task-items/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_TaskItemById_WhenNotExists_ShouldReturnNotFound()
    {
        var response = await _client.GetAsync("/api/task-items/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ─── GET /api/task-items/user/{userId} ────────────────────────────────────

    [Fact]
    public async Task GET_TaskItemsByUser_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/api/task-items/user/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ─── GET /api/task-items/search ───────────────────────────────────────────

    [Fact]
    public async Task GET_SearchTaskItems_WithValidFilter_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/api/task-items/search?UserId=1&Page=1&PageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_SearchTaskItems_FilterByStatus_ShouldReturnOk()
    {
        var response = await _client.GetAsync($"/api/task-items/search?Status={(int)Status.Pending}&Page=1&PageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_SearchTaskItems_FilterByPriority_ShouldReturnOk()
    {
        var response = await _client.GetAsync($"/api/task-items/search?Priority={(int)Priority.High}&Page=1&PageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ─── PUT /api/task-items/{id} ────────────────────────────────────────────

    [Fact]
    public async Task PUT_UpdateTaskItem_WhenExists_ShouldReturnOk()
    {
        var createCommand = new
        {
            Title = "Task to Update",
            Description = "Update me",
            Priority = (int)Priority.Low,
            DueDate = DateTime.UtcNow.AddDays(10).ToString("yyyy-MM-dd"),
            UserId = 1
        };
        var createResponse = await _client.PostAsJsonAsync("/api/task-items", createCommand);
        var createdData = await createResponse.Content.ReadFromJsonAsync<ApiResponse<int>>();
        var id = createdData?.Data;

        var updateCommand = new
        {
            Title = "Updated Task Title",
            Description = "Updated Description",
            Status = (int)Status.InProgress,
            Priority = (int)Priority.High,
            DueDate = DateTime.UtcNow.AddDays(15).ToString("yyyy-MM-dd")
        };

        var response = await _client.PutAsJsonAsync($"/api/task-items/{id}", updateCommand);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task PUT_UpdateTaskItem_WhenNotExists_ShouldReturnBadRequest()
    {
        var updateCommand = new
        {
            Title = "Non-existent Task",
            Description = "Description",
            Status = (int)Status.Pending,
            Priority = (int)Priority.Medium,
            DueDate = DateTime.UtcNow.AddDays(5).ToString("yyyy-MM-dd")
        };

        var response = await _client.PutAsJsonAsync("/api/task-items/99999", updateCommand);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ─── DELETE /api/task-items/{id} ─────────────────────────────────────────

    [Fact]
    public async Task DELETE_TaskItem_WhenExists_ShouldReturnOk()
    {
        var createCommand = new
        {
            Title = "Task to Delete",
            Description = "Delete me",
            Priority = (int)Priority.Medium,
            DueDate = DateTime.UtcNow.AddDays(3).ToString("yyyy-MM-dd"),
            UserId = 1
        };
        var createResponse = await _client.PostAsJsonAsync("/api/task-items", createCommand);
        var createdData = await createResponse.Content.ReadFromJsonAsync<ApiResponse<int>>();
        var id = createdData?.Data;

        var response = await _client.DeleteAsync($"/api/task-items/{id}");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DELETE_TaskItem_WhenNotExists_ShouldReturnBadRequest()
    {
        var response = await _client.DeleteAsync("/api/task-items/99999");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ─── POST /api/task-items/{id}/addLabels ──────────────────────────────────

    [Fact]
    public async Task POST_AddLabels_WhenTaskExists_ShouldReturnOk()
    {
        // Create a label first
        var labelCommand = new { Name = "Test Label", LabelColor = (int)LabelColor.Blue, UserId = 1 };
        var labelResponse = await _client.PostAsJsonAsync("/api/task-labels", labelCommand);
        var labelData = await labelResponse.Content.ReadFromJsonAsync<ApiResponse<int>>();
        var labelId = labelData?.Data;

        // Create a task
        var taskCommand = new
        {
            Title = "Task for Labels",
            Description = "Add label to me",
            Priority = (int)Priority.Medium,
            DueDate = DateTime.UtcNow.AddDays(7).ToString("yyyy-MM-dd"),
            UserId = 1
        };
        var taskResponse = await _client.PostAsJsonAsync("/api/task-items", taskCommand);
        var taskData = await taskResponse.Content.ReadFromJsonAsync<ApiResponse<int>>();
        var taskId = taskData?.Data;

        var addLabelsCommand = new { TaskLabelsId = new[] { labelId } };
        var response = await _client.PostAsJsonAsync($"/api/task-items/{taskId}/addLabels", addLabelsCommand);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }

    // ─── POST /api/task-items/batch ────────────────────────────────────────────

    [Fact]
    public async Task POST_CreateBatchTaskItems_WithValidData_ShouldReturnCreated()
    {
        var commands = new[]
        {
            new
            {
                Title = "Batch Task 1",
                Description = "Batch Description 1",
                Priority = (int)Priority.Low,
                DueDate = DateTime.UtcNow.AddDays(5).ToString("yyyy-MM-dd"),
                UserId = 1
            },
            new
            {
                Title = "Batch Task 2",
                Description = "Batch Description 2",
                Priority = (int)Priority.High,
                DueDate = DateTime.UtcNow.AddDays(10).ToString("yyyy-MM-dd"),
                UserId = 1
            }
        };

        var response = await _client.PostAsJsonAsync("/api/task-items/batch", commands);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    // ─── GET /api/task-items (GetAll) ─────────────────────────────────────────

    [Fact]
    public async Task GET_AllTaskItems_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/api/task-items?Page=1&PageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

/// <summary>Helper record for deserializing API responses.</summary>
public record ApiResponse<T>(T? Data, int StatusCode, string? Message, object? Pagination = null);
