using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.Category;
using Financial.Application.Features.Categories.Commands.Update;
using FluentAssertions;
using Moq;

namespace Financial.UnitTests.Application.Handlers.Commands.Categories
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Application")]
    public class UpdateCategoryCommandHandlerTests
    {
        private readonly Mock<ICategoryService> _serviceMock;
        private readonly UpdateCategoryCommandHandler _handler;

        public UpdateCategoryCommandHandlerTests()
        {
            _serviceMock = new Mock<ICategoryService>();
            _handler = new UpdateCategoryCommandHandler(_serviceMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldReturnSuccess()
        {
            // Arrange
            var command = new UpdateCategoryCommand(1, "Updated Name", "Updated Description");
            UpdateCategoryDTO? capturedDto = null;

            _serviceMock
                .Setup(x => x.UpdateAsync(It.IsAny<UpdateCategoryDTO>(), It.IsAny<CancellationToken>()))
                .Callback<UpdateCategoryDTO, CancellationToken>((dto, _) => capturedDto = dto)
                .ReturnsAsync(Result.Success(true));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.IsSuccess.Should().BeTrue();
            capturedDto.Should().NotBeNull();
            capturedDto!.Id.Should().Be(command.Id);
            capturedDto.Name.Should().Be(command.Name);
            capturedDto.Description.Should().Be(command.Description);
            _serviceMock.Verify(x => x.UpdateAsync(It.IsAny<UpdateCategoryDTO>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenServiceFails_ShouldReturnFailure()
        {
            // Arrange
            var command = new UpdateCategoryCommand(999, "Name", null);
            var error = Error.NotFound("Categoria nao encontrada");

            _serviceMock
                .Setup(x => x.UpdateAsync(It.IsAny<UpdateCategoryDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<bool>(error));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }
    }
}
