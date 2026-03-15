using Financial.Application.DTOs.Category;
using Financial.Application.Mappings;
using Financial.Domain.Entities;
using FluentAssertions;

namespace Financial.UnitTests.Application.Mappers
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Application")]
    public class CategoryMapperTests
    {
        [Fact]
        public void ToEntity_WithValidDTO_ShouldMapCorrectly()
        {
            // Arrange
            var dto = new CreateCategoryDTO(
                UserId: 1,
                Name: "Test Category",
                Description: "Test description");

            // Act
            var entity = dto.ToEntity();

            // Assert
            entity.Should().NotBeNull();
            entity.UserId.Should().Be(dto.UserId);
            entity.Name.Should().Be(dto.Name);
            entity.Description.Should().Be(dto.Description);
        }

        [Fact]
        public void ToEntity_WithNullDescription_ShouldMapCorrectly()
        {
            // Arrange
            var dto = new CreateCategoryDTO(
                UserId: 1,
                Name: "Test Category",
                Description: null);

            // Act
            var entity = dto.ToEntity();

            // Assert
            entity.Description.Should().BeNull();
        }

        [Fact]
        public void ToDTO_WithValidEntity_ShouldMapCorrectly()
        {
            // Arrange
            var entity = new Category(1, "Test Category", "Test description");

            // Act
            var dto = entity.ToDTO();

            // Assert
            dto.Should().NotBeNull();
            dto.UserId.Should().Be(entity.UserId);
            dto.Name.Should().Be(entity.Name);
            dto.Description.Should().Be(entity.Description);
            dto.CreatedAt.Should().Be(entity.CreatedAt);
            dto.UpdatedAt.Should().Be(entity.UpdatedAt);
        }

        [Fact]
        public void ToDTO_WithUpdatedEntity_ShouldMapUpdatedAt()
        {
            // Arrange
            var entity = new Category(1, "Old", "Old Description");
            entity.Update("New", "New Description");

            // Act
            var dto = entity.ToDTO();

            // Assert
            dto.UpdatedAt.Should().NotBeNull();
            dto.Name.Should().Be("New");
            dto.Description.Should().Be("New Description");
        }
    }
}
