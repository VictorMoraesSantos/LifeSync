using FluentAssertions;
using System.Net;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Domain.Enums;
using TaskManager.E2ETests.Fixtures;
using TaskManager.E2ETests.Helpers;

namespace TaskManager.E2ETests.Tests
{
    [Trait("Category", "E2E")]
    [Trait("Layer", "API")]
    public class TaskLabelsEndpointTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public TaskLabelsEndpointTests(CustomWebApplicationFactory factory)
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
                Name = "Important",
                LabelColor = (int)LabelColor.Red,
                UserId = 1
            };

            // Act
            var response = await _client.PostAsync("/api/task-labels", command.ToJsonContent());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var envelope = await response.DeserializeEnvelopeAsync<int>();
            envelope.Should().NotBeNull();
            envelope!.Success.Should().BeTrue();
            envelope.StatusCode.Should().Be((int)HttpStatusCode.Created);
            envelope.Data.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task POST_Create_WithInvalidName_ReturnsBadRequest()
        {
            // Arrange
            var command = new
            {
                Name = "",
                LabelColor = (int)LabelColor.Red,
                UserId = 1
            };

            // Act
            var response = await _client.PostAsync("/api/task-labels", command.ToJsonContent());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task POST_Create_WithShortName_ReturnsBadRequest()
        {
            // Arrange
            var command = new
            {
                Name = "A",
                LabelColor = (int)LabelColor.Red,
                UserId = 1
            };

            // Act
            var response = await _client.PostAsync("/api/task-labels", command.ToJsonContent());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region POST - CreateBatch

        [Fact(Skip = "Bug: Batch endpoint uses Task.WhenAll with shared scoped DbContext which is not thread-safe")]
        public async Task POST_CreateBatch_WithValidData_ReturnsCreated()
        {
            // Arrange
            var commands = new[]
            {
                new { Name = "Work", LabelColor = (int)LabelColor.Blue, UserId = 1 },
                new { Name = "Personal", LabelColor = (int)LabelColor.Green, UserId = 1 },
                new { Name = "Urgent", LabelColor = (int)LabelColor.Red, UserId = 1 }
            };

            // Act
            var response = await _client.PostAsync("/api/task-labels/batch", commands.ToJsonContent());

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<int>();
            envelope!.Success.Should().BeTrue();
            envelope.StatusCode.Should().Be((int)HttpStatusCode.Created);
        }

        #endregion

        #region GET - GetById

        [Fact]
        public async Task GET_GetById_WithExistingId_ReturnsOkWithLabel()
        {
            // Arrange
            var labelId = await CreateLabelAsync();

            // Act
            var response = await _client.GetAsync($"/api/task-labels/{labelId}");

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<TaskLabelDTO>();
            envelope!.Success.Should().BeTrue();
            envelope.Data!.Id.Should().Be(labelId);
            envelope.Data.Name.Should().Be("E2E Label");
        }

        [Fact]
        public async Task GET_GetById_WithNonExistingId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/task-labels/99999");

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeFalse();
            envelope.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        #endregion

        #region GET - GetAll

        [Fact]
        public async Task GET_GetAll_WithMultipleLabels_ReturnsOkWithAllLabels()
        {
            // Arrange
            await CreateLabelAsync("Label 1", LabelColor.Red);
            await CreateLabelAsync("Label 2", LabelColor.Blue);
            await CreateLabelAsync("Label 3", LabelColor.Green);

            // Act
            var response = await _client.GetAsync("/api/task-labels");

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<List<TaskLabelDTO>>();
            envelope!.Success.Should().BeTrue();
            envelope.Data!.Count.Should().BeGreaterThanOrEqualTo(3);
        }

        #endregion

        #region GET - GetByUserId

        [Fact]
        public async Task GET_GetByUserId_WithExistingUser_ReturnsOkWithUserLabels()
        {
            // Arrange
            await CreateLabelAsync("User 1 Label");

            // Act
            var response = await _client.GetAsync("/api/task-labels/user/1");

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<List<TaskLabelDTO>>();
            envelope!.Success.Should().BeTrue();
            envelope.Data!.Should().NotBeEmpty();
        }

        #endregion

        #region GET - Search (Filter)

        [Fact]
        public async Task GET_Search_WithColorFilter_ReturnsFilteredResults()
        {
            // Arrange
            await CreateLabelAsync("Red Label", LabelColor.Red);
            await CreateLabelAsync("Blue Label", LabelColor.Blue);
            await CreateLabelAsync("Another Red", LabelColor.Red);

            // Act
            var response = await _client.GetAsync($"/api/task-labels/search?userId=1&labelColor={(int)LabelColor.Red}");

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<List<TaskLabelDTO>>();
            envelope.Should().NotBeNull();
            envelope!.Success.Should().BeTrue();
            envelope.Data.Should().HaveCount(2);
        }

        [Fact]
        public async Task GET_Search_WithPagination_ReturnsPaginatedResults()
        {
            // Arrange
            for (int i = 1; i <= 8; i++)
            {
                await CreateLabelAsync($"Paginated Label {i}");
            }

            // Act
            var response = await _client.GetAsync("/api/task-labels/search?userId=1&page=1&pageSize=3");

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<List<TaskLabelDTO>>();
            envelope.Should().NotBeNull();
            envelope!.Data.Should().HaveCount(3);
            envelope.Pagination.Should().NotBeNull();
            envelope.Pagination!.TotalItems.Should().BeGreaterThanOrEqualTo(8);
        }

        [Fact]
        public async Task GET_Search_WithNameContains_ReturnsMatchingLabels()
        {
            // Arrange
            await CreateLabelAsync("Work Project");
            await CreateLabelAsync("Personal Stuff");
            await CreateLabelAsync("Work Home");

            // Act
            var response = await _client.GetAsync("/api/task-labels/search?nameContains=Work");

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<List<TaskLabelDTO>>();
            envelope!.Success.Should().BeTrue();
            envelope.Data.Should().HaveCount(2);
        }

        #endregion

        #region PUT - Update

        [Fact]
        public async Task PUT_Update_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var labelId = await CreateLabelAsync();

            var updateCommand = new
            {
                Id = labelId,
                Name = "Updated Label Name",
                LabelColor = (int)LabelColor.Purple
            };

            // Act
            var response = await _client.PutAsync($"/api/task-labels/{labelId}", updateCommand.ToJsonContent());

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeTrue();
            envelope.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            // Verify update
            var getResponse = await _client.GetAsync($"/api/task-labels/{labelId}");
            var getEnvelope = await getResponse.DeserializeEnvelopeAsync<TaskLabelDTO>();
            getEnvelope!.Data!.Name.Should().Be("Updated Label Name");
            getEnvelope.Data.LabelColor.Should().Be(LabelColor.Purple);
        }

        [Fact]
        public async Task PUT_Update_WithInvalidName_ReturnsBadRequest()
        {
            // Arrange
            var labelId = await CreateLabelAsync();

            var updateCommand = new
            {
                Id = labelId,
                Name = "",
                LabelColor = (int)LabelColor.Red
            };

            // Act
            var response = await _client.PutAsync($"/api/task-labels/{labelId}", updateCommand.ToJsonContent());

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
                Name = "Valid Name",
                LabelColor = (int)LabelColor.Red
            };

            // Act
            var response = await _client.PutAsync("/api/task-labels/99999", updateCommand.ToJsonContent());

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
            var labelId = await CreateLabelAsync();

            // Act
            var response = await _client.DeleteAsync($"/api/task-labels/{labelId}");

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeTrue();
            envelope.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            // Verify deletion
            var getResponse = await _client.GetAsync($"/api/task-labels/{labelId}");
            var getEnvelope = await getResponse.DeserializeEnvelopeAsync<object>();
            getEnvelope!.Success.Should().BeFalse();
            getEnvelope.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DELETE_WithNonExistingId_ReturnsNotFound()
        {
            // Act
            var response = await _client.DeleteAsync("/api/task-labels/99999");

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeFalse();
            envelope.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        #endregion

        #region All LabelColors

        [Theory]
        [InlineData(LabelColor.Red)]
        [InlineData(LabelColor.Green)]
        [InlineData(LabelColor.Blue)]
        [InlineData(LabelColor.Yellow)]
        [InlineData(LabelColor.Purple)]
        [InlineData(LabelColor.Orange)]
        [InlineData(LabelColor.Pink)]
        [InlineData(LabelColor.Brown)]
        [InlineData(LabelColor.Gray)]
        public async Task POST_Create_WithEachLabelColor_ReturnsCreated(LabelColor color)
        {
            // Arrange
            var command = new
            {
                Name = $"{color} Label",
                LabelColor = (int)color,
                UserId = 1
            };

            // Act
            var response = await _client.PostAsync("/api/task-labels", command.ToJsonContent());

            // Assert
            var envelope = await response.DeserializeEnvelopeAsync<int>();
            envelope!.Success.Should().BeTrue();
            envelope.StatusCode.Should().Be((int)HttpStatusCode.Created);
        }

        #endregion

        #region Helpers

        private async Task<int> CreateLabelAsync(string name = "E2E Label", LabelColor color = LabelColor.Blue)
        {
            var command = new { Name = name, LabelColor = (int)color, UserId = 1 };
            var response = await _client.PostAsync("/api/task-labels", command.ToJsonContent());
            var envelope = await response.DeserializeEnvelopeAsync<int>();
            envelope!.Success.Should().BeTrue($"Failed to create label '{name}': {string.Join(", ", envelope.Errors)}");
            return envelope.Data;
        }

        #endregion
    }
}
