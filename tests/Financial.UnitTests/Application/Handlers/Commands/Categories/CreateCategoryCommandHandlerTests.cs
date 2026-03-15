using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.Category;
using Financial.Application.Features.Categories.Commands.Create;
using FluentAssertions;
using Moq;

namespace Financial.UnitTests.Application.Handlers.Commands.Categories
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Application")]
    public class CreateCategoryCommandHandlerTests
    {
        private readonly Mock<ICategoryService> _serviceMock;
        private readonly CreateCategoryCommandHandler _handler;

        public CreateCategoryCommandHandlerTests()
        {
            _serviceMock = new Mock<ICategoryService>();
            _handler = new CreateCategoryCommandHandler(_serviceMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldReturnSuccess()
        {
            // Arrange
            var command = new CreateCategoryCommand(1, "Test Category", "Description");
            CreateCategoryDTO? capturedDto = null;

            _serviceMock
                .Setup(x => x.CreateAsync(It.IsAny<CreateCategoryDTO>(), It.IsAny<CancellationToken>()))
                .Callback<CreateCategoryDTO, CancellationToken>((dto, _) => capturedDto = dto)
                .ReturnsAsync(Result.Success(1));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(1);
            capturedDto.Should().NotBeNull();
            capturedDto!.UserId.Should().Be(command.UserId);
            capturedDto.Name.Should().Be(command.Name);
            capturedDto.Description.Should().Be(command.Description);
            _serviceMock.Verify(x => x.CreateAsync(It.IsAny<CreateCategoryDTO>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenServiceFails_ShouldReturnFailure()
        {
            // Arrange
            var command = new CreateCategoryCommand(1, "Test", "Desc");
            var error = Error.Failure("Erro ao criar categoria");

            _serviceMock
                .Setup(x => x.CreateAsync(It.IsAny<CreateCategoryDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<int>(error));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }
    }
}
