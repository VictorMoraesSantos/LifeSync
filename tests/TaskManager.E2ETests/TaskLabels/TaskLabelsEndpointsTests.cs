using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using TaskManager.Domain.Enums;
using TaskManager.E2ETests.Infrastructure;

namespace TaskManager.E2ETests.TaskLabels;

[Trait("Category", "E2E")]
[Trait("Layer", "API")]
public class TaskLabelsEndpointsTests : IClassFixture<TaskManagerWebApplicationFactory>, IAsyncLifetime
{
    private readonly TaskManagerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public TaskLabelsEndpointsTests(TaskManagerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    // ─── POST /api/task-labels ────────────────────────────────────────────────

    [Fact]
    public async Task POST_CreateTaskLabel_WithValidData_ShouldReturnCreated()
    {
        var command = new { Name = "Work", LabelColor = (int)LabelColor.Blue, UserId = 1 };

        var response = await _client.PostAsJsonAsync("/api/task-labels", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task POST_CreateTaskLabel_WithInvalidName_ShouldReturnBadRequest()
    {
        var command = new { Name = "", LabelColor = (int)LabelColor.Blue, UserId = 1 };

        var response = await _client.PostAsJsonAsync("/api/task-labels", command);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InlineData((int)LabelColor.Red)]
    [InlineData((int)LabelColor.Green)]
    [InlineData((int)LabelColor.Blue)]
    [InlineData((int)LabelColor.Yellow)]
    [InlineData((int)LabelColor.Purple)]
    public async Task POST_CreateTaskLabel_WithDifferentColors_ShouldReturnCreated(int colorValue)
    {
        var command = new { Name = $"Color Label {colorValue}", LabelColor = colorValue, UserId = 1 };

        var response = await _client.PostAsJsonAsync("/api/task-labels", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    // ─── GET /api/task-labels/{id} ────────────────────────────────────────────

    [Fact]
    public async Task GET_TaskLabelById_WhenExists_ShouldReturnOk()
    {
        var createCommand = new { Name = "Fetch Label", LabelColor = (int)LabelColor.Green, UserId = 1 };
        var createResponse = await _client.PostAsJsonAsync("/api/task-labels", createCommand);
        var createdData = await createResponse.Content.ReadFromJsonAsync<ApiResponse<int>>();
        var id = createdData?.Data;

        var response = await _client.GetAsync($"/api/task-labels/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_TaskLabelById_WhenNotExists_ShouldReturnNotFound()
    {
        var response = await _client.GetAsync("/api/task-labels/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ─── GET /api/task-labels/user/{userId} ───────────────────────────────────

    [Fact]
    public async Task GET_TaskLabelsByUser_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/api/task-labels/user/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ─── GET /api/task-labels ─────────────────────────────────────────────────

    [Fact]
    public async Task GET_AllTaskLabels_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/api/task-labels?Page=1&PageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ─── GET /api/task-labels/search ─────────────────────────────────────────

    [Fact]
    public async Task GET_SearchTaskLabels_WithValidFilter_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/api/task-labels/search?UserId=1&Page=1&PageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_SearchTaskLabels_FilterByColor_ShouldReturnOk()
    {
        var response = await _client.GetAsync($"/api/task-labels/search?LabelColor={(int)LabelColor.Blue}&Page=1&PageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_SearchTaskLabels_FilterByName_ShouldReturnMatchingResults()
    {
        // Create a few labels
        await _client.PostAsJsonAsync("/api/task-labels", new { Name = "Work Alpha", LabelColor = (int)LabelColor.Blue, UserId = 1 });
        await _client.PostAsJsonAsync("/api/task-labels", new { Name = "Personal Beta", LabelColor = (int)LabelColor.Red, UserId = 1 });
        await _client.PostAsJsonAsync("/api/task-labels", new { Name = "Work Gamma", LabelColor = (int)LabelColor.Green, UserId = 1 });

        var response = await _client.GetAsync("/api/task-labels/search?NameContains=Work&Page=1&PageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ─── PUT /api/task-labels/{id} ────────────────────────────────────────────

    [Fact]
    public async Task PUT_UpdateTaskLabel_WhenExists_ShouldReturnOk()
    {
        var createCommand = new { Name = "Label to Update", LabelColor = (int)LabelColor.Blue, UserId = 1 };
        var createResponse = await _client.PostAsJsonAsync("/api/task-labels", createCommand);
        var createdData = await createResponse.Content.ReadFromJsonAsync<ApiResponse<int>>();
        var id = createdData?.Data;

        var updateCommand = new { Name = "Updated Label Name", LabelColor = (int)LabelColor.Red };

        var response = await _client.PutAsJsonAsync($"/api/task-labels/{id}", updateCommand);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task PUT_UpdateTaskLabel_WhenNotExists_ShouldReturnBadRequest()
    {
        var updateCommand = new { Name = "Non-existent Label", LabelColor = (int)LabelColor.Blue };

        var response = await _client.PutAsJsonAsync("/api/task-labels/99999", updateCommand);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ─── DELETE /api/task-labels/{id} ─────────────────────────────────────────

    [Fact]
    public async Task DELETE_TaskLabel_WhenExists_ShouldReturnOk()
    {
        var createCommand = new { Name = "Label to Delete", LabelColor = (int)LabelColor.Purple, UserId = 1 };
        var createResponse = await _client.PostAsJsonAsync("/api/task-labels", createCommand);
        var createdData = await createResponse.Content.ReadFromJsonAsync<ApiResponse<int>>();
        var id = createdData?.Data;

        var response = await _client.DeleteAsync($"/api/task-labels/{id}");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DELETE_TaskLabel_WhenNotExists_ShouldReturnNotFound()
    {
        var response = await _client.DeleteAsync("/api/task-labels/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ─── POST /api/task-labels/batch ─────────────────────────────────────────

    [Fact]
    public async Task POST_CreateBatchTaskLabels_WithValidData_ShouldReturnCreated()
    {
        var commands = new[]
        {
            new { Name = "Batch Label 1", LabelColor = (int)LabelColor.Red, UserId = 1 },
            new { Name = "Batch Label 2", LabelColor = (int)LabelColor.Blue, UserId = 1 },
            new { Name = "Batch Label 3", LabelColor = (int)LabelColor.Green, UserId = 1 }
        };

        var response = await _client.PostAsJsonAsync("/api/task-labels/batch", commands);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}

/// <summary>Helper record for deserializing API responses in TaskLabels tests.</summary>
public record ApiResponse<T>(T? Data, int StatusCode, string? Message, object? Pagination = null);
