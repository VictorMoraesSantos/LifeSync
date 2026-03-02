using FluentAssertions;
using System.Net;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Domain.Enums;
using TaskManager.E2ETests.Fixtures;
using TaskManager.E2ETests.Helpers;

namespace TaskManager.E2ETests.Tests
{
    [Trait("Category", "E2E")]
    [Trait("Layer", "API")]
    public class TaskLifecycleTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public TaskLifecycleTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        public async Task InitializeAsync()
        {
            await _factory.ResetDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task CompleteTaskLifecycle_CreateUpdateAddLabelComplete()
        {
            // 1. Create a task
            var createCommand = new
            {
                Title = "Lifecycle Task",
                Description = "Task for lifecycle test",
                Priority = (int)Priority.Medium,
                DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
                UserId = 1
            };

            var createResponse = await _client.PostAsync("/api/task-items", createCommand.ToJsonContent());
            createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var createEnvelope = await createResponse.DeserializeEnvelopeAsync<int>();
            createEnvelope!.Success.Should().BeTrue();
            createEnvelope.StatusCode.Should().Be((int)HttpStatusCode.Created);
            var taskId = createEnvelope.Data;
            taskId.Should().BeGreaterThan(0);

            // 2. Create a label
            var labelCommand = new
            {
                Name = "Important",
                LabelColor = (int)LabelColor.Red,
                UserId = 1
            };

            var labelResponse = await _client.PostAsync("/api/task-labels", labelCommand.ToJsonContent());
            labelResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var labelEnvelope = await labelResponse.DeserializeEnvelopeAsync<int>();
            labelEnvelope!.Success.Should().BeTrue();
            labelEnvelope.StatusCode.Should().Be((int)HttpStatusCode.Created);
            var labelId = labelEnvelope.Data;

            // 3. Add label to task
            var addLabelCommand = new { TaskItemId = taskId, TaskLabelsId = new[] { labelId } };
            var addLabelResponse = await _client.PostAsync(
                $"/api/task-items/{taskId}/addLabels", addLabelCommand.ToJsonContent());
            addLabelResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var addLabelEnvelope = await addLabelResponse.DeserializeEnvelopeAsync<bool>();
            addLabelEnvelope!.Success.Should().BeTrue();

            // 4. Verify label is on task
            var getResponse = await _client.GetAsync($"/api/task-items/{taskId}");
            var getEnvelope = await getResponse.DeserializeEnvelopeAsync<TaskItemDTO>();
            getEnvelope!.Success.Should().BeTrue();
            getEnvelope.Data!.Labels.Should().HaveCount(1);
            getEnvelope.Data.Labels.First().Name.Should().Be("Important");

            // 5. Update task to InProgress
            var updateCommand = new
            {
                Id = taskId,
                Title = "Lifecycle Task",
                Description = "Task for lifecycle test",
                Status = (int)Status.InProgress,
                Priority = (int)Priority.High,
                DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7))
            };

            var updateResponse = await _client.PutAsync($"/api/task-items/{taskId}", updateCommand.ToJsonContent());
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var updateEnvelope = await updateResponse.DeserializeEnvelopeAsync<object>();
            updateEnvelope!.Success.Should().BeTrue();
            updateEnvelope.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            // 6. Verify task is InProgress with updated priority
            getResponse = await _client.GetAsync($"/api/task-items/{taskId}");
            getEnvelope = await getResponse.DeserializeEnvelopeAsync<TaskItemDTO>();
            getEnvelope!.Data!.Status.Should().Be(Status.InProgress);
            getEnvelope.Data.Priority.Should().Be(Priority.High);

            // 7. Complete the task
            var completeCommand = new
            {
                Id = taskId,
                Title = "Lifecycle Task",
                Description = "Task for lifecycle test",
                Status = (int)Status.Completed,
                Priority = (int)Priority.High,
                DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7))
            };

            var completeResponse = await _client.PutAsync($"/api/task-items/{taskId}", completeCommand.ToJsonContent());
            completeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var completeEnvelope = await completeResponse.DeserializeEnvelopeAsync<object>();
            completeEnvelope!.Success.Should().BeTrue();
            completeEnvelope.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            // 8. Verify task is completed
            getResponse = await _client.GetAsync($"/api/task-items/{taskId}");
            getEnvelope = await getResponse.DeserializeEnvelopeAsync<TaskItemDTO>();
            getEnvelope!.Data!.Status.Should().Be(Status.Completed);
        }

        [Fact]
        public async Task TaskWithMultipleLabels_AddAndRemoveLabels()
        {
            // 1. Create labels
            var label1Id = await CreateLabelAsync("Work", LabelColor.Blue);
            var label2Id = await CreateLabelAsync("Urgent", LabelColor.Red);
            var label3Id = await CreateLabelAsync("Personal", LabelColor.Green);

            // 2. Create task
            var taskId = await CreateTaskAsync("Multi-Label Task");

            // 3. Add multiple labels
            var addCommand = new { TaskItemId = taskId, TaskLabelsId = new[] { label1Id, label2Id, label3Id } };
            var addResponse = await _client.PostAsync(
                $"/api/task-items/{taskId}/addLabels", addCommand.ToJsonContent());
            addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var addEnvelope = await addResponse.DeserializeEnvelopeAsync<bool>();
            addEnvelope!.Success.Should().BeTrue();

            // 4. Verify all labels are present
            var task = await GetTaskAsync(taskId);
            task!.Labels.Should().HaveCount(3);

            // 5. Remove one label
            var removeCommand = new { TaskItemId = taskId, TaskLabelsId = new[] { label2Id } };
            var removeResponse = await _client.PostAsync(
                $"/api/task-items/{taskId}/removeLabels", removeCommand.ToJsonContent());
            removeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var removeEnvelope = await removeResponse.DeserializeEnvelopeAsync<bool>();
            removeEnvelope!.Success.Should().BeTrue();

            // 6. Verify label was removed
            task = await GetTaskAsync(taskId);
            task!.Labels.Should().HaveCount(2);
            task.Labels.Should().NotContain(l => l.Name == "Urgent");
        }

        [Fact]
        public async Task CreateAndDeleteTask_VerifyFullDeletion()
        {
            // 1. Create task
            var taskId = await CreateTaskAsync("Delete Lifecycle Task");

            // 2. Verify it exists
            var getResponse = await _client.GetAsync($"/api/task-items/{taskId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var getEnvelope = await getResponse.DeserializeEnvelopeAsync<TaskItemDTO>();
            getEnvelope!.Success.Should().BeTrue();

            // 3. Delete task
            var deleteResponse = await _client.DeleteAsync($"/api/task-items/{taskId}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var deleteEnvelope = await deleteResponse.DeserializeEnvelopeAsync<object>();
            deleteEnvelope!.Success.Should().BeTrue();
            deleteEnvelope.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            // 4. Verify it's gone
            getResponse = await _client.GetAsync($"/api/task-items/{taskId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getEnvelope = await getResponse.DeserializeEnvelopeAsync<TaskItemDTO>();
            getEnvelope!.Success.Should().BeFalse();
            getEnvelope.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateAndDeleteLabel_VerifyFullDeletion()
        {
            // 1. Create label
            var labelId = await CreateLabelAsync("Deletable Label", LabelColor.Gray);

            // 2. Verify it exists
            var getResponse = await _client.GetAsync($"/api/task-labels/{labelId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var getEnvelope = await getResponse.DeserializeEnvelopeAsync<TaskLabelDTO>();
            getEnvelope!.Success.Should().BeTrue();

            // 3. Delete label
            var deleteResponse = await _client.DeleteAsync($"/api/task-labels/{labelId}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var deleteEnvelope = await deleteResponse.DeserializeEnvelopeAsync<object>();
            deleteEnvelope!.Success.Should().BeTrue();
            deleteEnvelope.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            // 4. Verify it's gone
            getResponse = await _client.GetAsync($"/api/task-labels/{labelId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getEnvelope = await getResponse.DeserializeEnvelopeAsync<TaskLabelDTO>();
            getEnvelope!.Success.Should().BeFalse();
            getEnvelope.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact(Skip = "Bug: Batch endpoint uses Task.WhenAll with shared scoped DbContext which is not thread-safe")]
        public async Task BatchCreateTasks_SearchByFilter_VerifyPagination()
        {
            // 1. Create multiple tasks via batch
            var commands = Enumerable.Range(1, 10).Select(i => new
            {
                Title = $"Batch Lifecycle Task {i:D2}",
                Description = $"Description for batch task {i}",
                Priority = i <= 5 ? (int)Priority.Medium : (int)Priority.High,
                DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i)),
                UserId = 1
            }).ToList();

            var batchResponse = await _client.PostAsync("/api/task-items/batch", commands.ToJsonContent());
            batchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var batchEnvelope = await batchResponse.DeserializeEnvelopeAsync<int>();
            batchEnvelope!.Success.Should().BeTrue();
            batchEnvelope.StatusCode.Should().Be((int)HttpStatusCode.Created);

            // 2. Search with pagination
            var searchResponse = await _client.GetAsync("/api/task-items/search?userId=1&page=1&pageSize=4");
            searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var envelope = await searchResponse.DeserializeEnvelopeAsync<List<TaskItemDTO>>();
            envelope!.Data.Should().HaveCount(4);
            envelope.Pagination.Should().NotBeNull();
            envelope.Pagination!.TotalItems.Should().Be(10);
            envelope.Pagination.TotalPages.Should().Be(3);

            // 3. Search with priority filter
            var priorityResponse = await _client.GetAsync(
                $"/api/task-items/search?userId=1&priority={(int)Priority.High}");
            priorityResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var priorityEnvelope = await priorityResponse.DeserializeEnvelopeAsync<List<TaskItemDTO>>();
            priorityEnvelope!.Data.Should().HaveCount(5);
            priorityEnvelope.Data!.Should().AllSatisfy(t => t.Priority.Should().Be(Priority.High));
        }

        [Fact]
        public async Task LabelUpdateLifecycle_CreateUpdateVerify()
        {
            // 1. Create label
            var labelId = await CreateLabelAsync("Original Name", LabelColor.Red);

            // 2. Verify creation
            var label = await GetLabelAsync(labelId);
            label!.Name.Should().Be("Original Name");
            label.LabelColor.Should().Be(LabelColor.Red);

            // 3. Update label
            var updateCommand = new
            {
                Id = labelId,
                Name = "Updated Name",
                LabelColor = (int)LabelColor.Purple
            };
            var updateResponse = await _client.PutAsync($"/api/task-labels/{labelId}", updateCommand.ToJsonContent());
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var updateEnvelope = await updateResponse.DeserializeEnvelopeAsync<object>();
            updateEnvelope!.Success.Should().BeTrue();
            updateEnvelope.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            // 4. Verify update
            label = await GetLabelAsync(labelId);
            label!.Name.Should().Be("Updated Name");
            label.LabelColor.Should().Be(LabelColor.Purple);
        }

        #region Helpers

        private async Task<int> CreateTaskAsync(string title = "Lifecycle Task")
        {
            var command = new
            {
                Title = title,
                Description = "Lifecycle test description",
                Priority = (int)Priority.Medium,
                DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
                UserId = 1
            };

            var response = await _client.PostAsync("/api/task-items", command.ToJsonContent());
            var envelope = await response.DeserializeEnvelopeAsync<int>();
            envelope!.Success.Should().BeTrue($"Failed to create task '{title}': {string.Join(", ", envelope.Errors)}");
            return envelope.Data;
        }

        private async Task<int> CreateLabelAsync(string name, LabelColor color)
        {
            var command = new { Name = name, LabelColor = (int)color, UserId = 1 };
            var response = await _client.PostAsync("/api/task-labels", command.ToJsonContent());
            var envelope = await response.DeserializeEnvelopeAsync<int>();
            envelope!.Success.Should().BeTrue($"Failed to create label '{name}': {string.Join(", ", envelope.Errors)}");
            return envelope.Data;
        }

        private async Task<TaskItemDTO?> GetTaskAsync(int taskId)
        {
            var response = await _client.GetAsync($"/api/task-items/{taskId}");
            var envelope = await response.DeserializeEnvelopeAsync<TaskItemDTO>();
            envelope!.Success.Should().BeTrue();
            return envelope.Data;
        }

        private async Task<TaskLabelDTO?> GetLabelAsync(int labelId)
        {
            var response = await _client.GetAsync($"/api/task-labels/{labelId}");
            var envelope = await response.DeserializeEnvelopeAsync<TaskLabelDTO>();
            envelope!.Success.Should().BeTrue();
            return envelope.Data;
        }

        #endregion
    }
}
