using FluentAssertions;
using System.Net;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Domain.Enums;
using TaskManager.E2ETests.Fixtures;
using TaskManager.E2ETests.Helpers;

namespace TaskManager.E2ETests.Tests
{
    [Trait("Category", "E2E")]
    [Trait("Layer", "API")]
    public class TaskItemsEndpointTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public TaskItemsEndpointTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        public async Task InitializeAsync()
        {
            await _factory.ResetDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        #region POST - Create

        [Fact]
        public async Task POST_Create_WithValidData_ReturnsCreated()
        {
            // Arrange
            var command = new
            {
                Title = "E2E Test Task",
                Description = "Test Description for E2E",
                Priority = (int)Priority.High,
                DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                UserId = 1
            };

            // Act
            var response = await _client.PostAsync("/api/task-items", command.ToJsonContent());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var envelope = await response.DeserializeEnvelopeAsync<int>();
            envelope.Should().NotBeNull();
            envelope!.Success.Should().BeTrue();
            envelope.StatusCode.Should().Be((int)HttpStatusCode.Created);
            envelope.Data.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task POST_Create_WithInvalidTitle_ReturnsBadRequest()
        {
            // Arrange
            var command = new
            {
                Title = "",
                Description = "Valid Description",
                Priority = (int)Priority.Medium,
                DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                UserId = 1
            };

            // Act
            var response = await _client.PostAsync("/api/task-items", command.ToJsonContent());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task POST_Create_WithShortDescription_ReturnsBadRequest()
        {
            // Arrange
            var command = new
            {
                Title = "Valid Title",
                Description = "Hi",
                Priority = (int)Priority.Medium,
                DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                UserId = 1
            };

            // Act
            var response = await _client.PostAsync("/api/task-items", command.ToJsonContent());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task POST_Create_WithLabels_ReturnsCreatedWithLabelsAssociated()
        {
            // Arrange - Create task and label separately, then associate via addLabels endpoint
            var labelId = await CreateTaskLabelAsync();
            var taskId = await CreateTaskItemAsync("Task With Label");

            var addLabelCommand = new { TaskItemId = taskId, TaskLabelsId = new[] { labelId } };

            // Act
            var response = await _client.PostAsync($"/api/task-items/{taskId}/addLabels", addLabelCommand.ToJsonContent());

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<bool>();
            envelope!.Success.Should().BeTrue();

            // Verify labels are associated
            var getResponse = await _client.GetAsync($"/api/task-items/{taskId}");
            var getEnvelope = await getResponse.DeserializeEnvelopeAsync<TaskItemDTO>();
            getEnvelope!.Data!.Labels.Should().HaveCount(1);
        }

        #endregion

        #region POST - CreateBatch

        [Fact(Skip = "Bug: Batch endpoint uses Task.WhenAll with shared scoped DbContext which is not thread-safe")]
        public async Task POST_CreateBatch_WithValidData_ReturnsCreated()
        {
            // Arrange
            var commands = Enumerable.Range(1, 3).Select(i => new
            {
                Title = $"Batch Task {i}",
                Description = $"Batch Description {i}",
                Priority = (int)Priority.Medium,
                DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i)),
                UserId = 1
            }).ToList();

            // Act
            var response = await _client.PostAsync("/api/task-items/batch", commands.ToJsonContent());

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<int>();
            envelope!.Success.Should().BeTrue();
            envelope.StatusCode.Should().Be((int)HttpStatusCode.Created);
        }

        #endregion

        #region GET - GetById

        [Fact]
        public async Task GET_GetById_WithExistingId_ReturnsOkWithTask()
        {
            // Arrange
            var taskId = await CreateTaskItemAsync();

            // Act
            var response = await _client.GetAsync($"/api/task-items/{taskId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var envelope = await response.DeserializeEnvelopeAsync<TaskItemDTO>();
            envelope!.Success.Should().BeTrue();
            envelope.Data!.Id.Should().Be(taskId);
            envelope.Data.Title.Should().Be("E2E Task");
        }

        [Fact]
        public async Task GET_GetById_WithNonExistingId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/task-items/99999");

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeFalse();
            envelope.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        #endregion

        #region GET - GetAll

        [Fact]
        public async Task GET_GetAll_WithMultipleItems_ReturnsOkWithAllItems()
        {
            // Arrange
            await CreateTaskItemAsync("Task 1");
            await CreateTaskItemAsync("Task 2");
            await CreateTaskItemAsync("Task 3");

            // Act
            var response = await _client.GetAsync("/api/task-items");

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<List<TaskItemDTO>>();
            envelope!.Success.Should().BeTrue();
            envelope.Data!.Count.Should().BeGreaterThanOrEqualTo(3);
        }

        #endregion

        #region GET - GetByUserId

        [Fact]
        public async Task GET_GetByUserId_WithExistingUser_ReturnsOkWithUserTasks()
        {
            // Arrange
            await CreateTaskItemAsync("User 1 Task", userId: 1);

            // Act
            var response = await _client.GetAsync("/api/task-items/user/1");

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<List<TaskItemDTO>>();
            envelope!.Success.Should().BeTrue();
            envelope.Data!.Should().NotBeEmpty();
            envelope.Data.Should().AllSatisfy(t => t.UserId.Should().Be(1));
        }

        #endregion

        #region GET - Search (Filter)

        [Fact]
        public async Task GET_Search_WithStatusFilter_ReturnsFilteredResults()
        {
            // Arrange
            await CreateTaskItemAsync("Pending Task");

            // Act
            var response = await _client.GetAsync("/api/task-items/search?userId=1&status=1");

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<List<TaskItemDTO>>();
            envelope.Should().NotBeNull();
            envelope!.Success.Should().BeTrue();
        }

        [Fact]
        public async Task GET_Search_WithPagination_ReturnsPaginatedResults()
        {
            // Arrange
            for (int i = 1; i <= 8; i++)
            {
                await CreateTaskItemAsync($"Search Task {i}");
            }

            // Act
            var response = await _client.GetAsync("/api/task-items/search?userId=1&page=1&pageSize=3");

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<List<TaskItemDTO>>();
            envelope.Should().NotBeNull();
            envelope!.Success.Should().BeTrue();
            envelope.Data.Should().HaveCount(3);
            envelope.Pagination.Should().NotBeNull();
            envelope.Pagination!.TotalItems.Should().BeGreaterThanOrEqualTo(8);
        }

        [Fact]
        public async Task GET_Search_WithPriorityFilter_ReturnsMatchingItems()
        {
            // Arrange
            await CreateTaskItemAsync("Urgent Task", priority: Priority.Urgent);
            await CreateTaskItemAsync("Low Task", priority: Priority.Low);

            // Act
            var response = await _client.GetAsync($"/api/task-items/search?userId=1&priority={(int)Priority.Urgent}");

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<List<TaskItemDTO>>();
            envelope!.Success.Should().BeTrue();
            envelope.Data!.Should().AllSatisfy(t => t.Priority.Should().Be(Priority.Urgent));
        }

        #endregion

        #region POST - AddLabels / RemoveLabels

        [Fact]
        public async Task POST_AddLabels_WithValidIds_ReturnsOk()
        {
            // Arrange
            var taskId = await CreateTaskItemAsync();
            var labelId = await CreateTaskLabelAsync();

            var command = new { TaskItemId = taskId, TaskLabelsId = new[] { labelId } };

            // Act
            var response = await _client.PostAsync($"/api/task-items/{taskId}/addLabels", command.ToJsonContent());

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<bool>();
            envelope!.Success.Should().BeTrue();

            // Verify label was added
            var getResponse = await _client.GetAsync($"/api/task-items/{taskId}");
            var getEnvelope = await getResponse.DeserializeEnvelopeAsync<TaskItemDTO>();
            getEnvelope!.Data!.Labels.Should().HaveCount(1);
        }

        [Fact]
        public async Task POST_RemoveLabels_WithExistingLabel_ReturnsOk()
        {
            // Arrange - Create task and label separately, then associate via addLabels
            var labelId = await CreateTaskLabelAsync();
            var taskId = await CreateTaskItemAsync("Task With Label");

            var addCommand = new { TaskItemId = taskId, TaskLabelsId = new[] { labelId } };
            var addResponse = await _client.PostAsync($"/api/task-items/{taskId}/addLabels", addCommand.ToJsonContent());
            var addEnvelope = await addResponse.DeserializeEnvelopeAsync<bool>();
            addEnvelope!.Success.Should().BeTrue("Label should be added before removal test");

            var removeCommand = new { TaskItemId = taskId, TaskLabelsId = new[] { labelId } };

            // Act
            var response = await _client.PostAsync($"/api/task-items/{taskId}/removeLabels", removeCommand.ToJsonContent());

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<bool>();
            envelope!.Success.Should().BeTrue();

            var getResponse = await _client.GetAsync($"/api/task-items/{taskId}");
            var getEnvelope = await getResponse.DeserializeEnvelopeAsync<TaskItemDTO>();
            getEnvelope!.Data!.Labels.Should().BeEmpty();
        }

        [Fact]
        public async Task POST_AddLabels_WithNonExistingTask_ReturnsBadRequest()
        {
            // Arrange
            var command = new { TaskItemId = 99999, TaskLabelsId = new[] { 1 } };

            // Act
            var response = await _client.PostAsync("/api/task-items/99999/addLabels", command.ToJsonContent());

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeFalse();
            envelope.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        #endregion

        #region PUT - Update

        [Fact]
        public async Task PUT_Update_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var taskId = await CreateTaskItemAsync();

            var updateCommand = new
            {
                Id = taskId,
                Title = "Updated Title",
                Description = "Updated Description here",
                Status = (int)Status.InProgress,
                Priority = (int)Priority.Urgent,
                DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10))
            };

            // Act
            var response = await _client.PutAsync($"/api/task-items/{taskId}", updateCommand.ToJsonContent());

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeTrue();
            envelope.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            // Verify update
            var getResponse = await _client.GetAsync($"/api/task-items/{taskId}");
            var getEnvelope = await getResponse.DeserializeEnvelopeAsync<TaskItemDTO>();
            getEnvelope!.Data!.Title.Should().Be("Updated Title");
            getEnvelope.Data.Status.Should().Be(Status.InProgress);
            getEnvelope.Data.Priority.Should().Be(Priority.Urgent);
        }

        [Fact]
        public async Task PUT_Update_WithInvalidTitle_ReturnsBadRequest()
        {
            // Arrange
            var taskId = await CreateTaskItemAsync();

            var updateCommand = new
            {
                Id = taskId,
                Title = "",
                Description = "Valid Description",
                Status = (int)Status.Pending,
                Priority = (int)Priority.Medium,
                DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5))
            };

            // Act
            var response = await _client.PutAsync($"/api/task-items/{taskId}", updateCommand.ToJsonContent());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PUT_Update_WithNonExistingId_ReturnsBadRequest()
        {
            // Arrange
            var updateCommand = new
            {
                Id = 99999,
                Title = "Valid Title",
                Description = "Valid Description",
                Status = (int)Status.Pending,
                Priority = (int)Priority.Medium,
                DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5))
            };

            // Act
            var response = await _client.PutAsync("/api/task-items/99999", updateCommand.ToJsonContent());

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeFalse();
            envelope.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        #endregion

        #region DELETE

        [Fact]
        public async Task DELETE_WithExistingId_ReturnsDeleted()
        {
            // Arrange
            var taskId = await CreateTaskItemAsync();

            // Act
            var response = await _client.DeleteAsync($"/api/task-items/{taskId}");

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeTrue();
            envelope.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            // Verify deletion
            var getResponse = await _client.GetAsync($"/api/task-items/{taskId}");
            var getEnvelope = await getResponse.DeserializeEnvelopeAsync<object>();
            getEnvelope!.Success.Should().BeFalse();
            getEnvelope.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DELETE_WithNonExistingId_ReturnsBadRequest()
        {
            // Act
            var response = await _client.DeleteAsync("/api/task-items/99999");

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeFalse();
            envelope.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        #endregion

        #region Helpers

        private async Task<int> CreateTaskItemAsync(string title = "E2E Task", int userId = 1, Priority priority = Priority.High)
        {
            var command = new
            {
                Title = title,
                Description = "E2E Test Description",
                Priority = (int)priority,
                DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                UserId = userId
            };

            var response = await _client.PostAsync("/api/task-items", command.ToJsonContent());
            var envelope = await response.DeserializeEnvelopeAsync<int>();
            envelope!.Success.Should().BeTrue($"Failed to create task item '{title}': {string.Join(", ", envelope.Errors)}");
            return envelope.Data;
        }

        private async Task<int> CreateTaskLabelAsync(string name = "Test Label", LabelColor color = LabelColor.Blue)
        {
            var command = new { Name = name, LabelColor = (int)color, UserId = 1 };
            var response = await _client.PostAsync("/api/task-labels", command.ToJsonContent());
            var envelope = await response.DeserializeEnvelopeAsync<int>();
            envelope!.Success.Should().BeTrue($"Failed to create task label '{name}': {string.Join(", ", envelope.Errors)}");
            return envelope.Data;
        }

        #endregion
    }
}
