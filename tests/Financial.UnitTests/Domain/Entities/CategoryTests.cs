using Core.Domain.Exceptions;
using Financial.Domain.Entities;
using Financial.Domain.Errors;
using Financial.UnitTests.Helpers.Builders;
using FluentAssertions;

namespace Financial.UnitTests.Domain.Entities
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Domain")]
    public class CategoryTests
    {
        #region Constructor Tests

        [Fact]
        public void Create_WithValidParameters_ShouldCreateEntity()
        {
            // Arrange
            var userId = 1;
            var name = "Alimentacao";
            var description = "Gastos com comida";

            // Act
            var category = new Category(userId, name, description);

            // Assert
            category.Should().NotBeNull();
            category.UserId.Should().Be(userId);
            category.Name.Should().Be(name);
            category.Description.Should().Be(description);
        }

        [Fact]
        public void Create_WithoutDescription_ShouldCreateWithNullDescription()
        {
            // Act
            var category = new Category(1, "Lazer");

            // Assert
            category.UserId.Should().Be(1);
            category.Name.Should().Be("Lazer");
            category.Description.Should().BeNull();
        }

        [Fact]
        public void Create_ShouldSetCreatedAt()
        {
            // Arrange
            var before = DateTime.UtcNow;

            // Act
            var category = CategoryBuilder.Default().Build();

            // Assert
            category.CreatedAt.Should().BeOnOrAfter(before);
            category.UpdatedAt.Should().BeNull();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Create_WithInvalidUserId_ShouldThrowDomainException(int invalidUserId)
        {
            // Act
            var act = () => new Category(invalidUserId, "Test");

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage(CategoryErrors.InvalidUserId.Description);
        }

        [Fact]
        public void Create_WithNullName_ShouldThrowDomainException()
        {
            // Act
            var act = () => new Category(1, null!);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage(CategoryErrors.InvalidName.Description);
        }

        [Fact]
        public void Create_WithEmptyName_ShouldThrowDomainException()
        {
            // Act
            var act = () => new Category(1, string.Empty);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage(CategoryErrors.InvalidName.Description);
        }

        [Fact]
        public void Create_WithWhitespaceName_ShouldThrowDomainException()
        {
            // Act
            var act = () => new Category(1, "   ");

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage(CategoryErrors.InvalidName.Description);
        }

        #endregion

        #region Update Tests

        [Fact]
        public void Update_WithValidParameters_ShouldUpdateProperties()
        {
            // Arrange
            var category = CategoryBuilder.Default().Build();
            var newName = "Updated Name";
            var newDescription = "Updated Description";

            // Act
            category.Update(newName, newDescription);

            // Assert
            category.Name.Should().Be(newName);
            category.Description.Should().Be(newDescription);
            category.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Update_WithNullDescription_ShouldSetDescriptionToNull()
        {
            // Arrange
            var category = CategoryBuilder.Default()
                .WithDescription("Old Description")
                .Build();

            // Act
            category.Update("New Name", null);

            // Assert
            category.Name.Should().Be("New Name");
            category.Description.Should().BeNull();
        }

        [Fact]
        public void Update_WithNullName_ShouldThrowDomainException()
        {
            // Arrange
            var category = CategoryBuilder.Default().Build();

            // Act
            var act = () => category.Update(null!);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage(CategoryErrors.InvalidName.Description);
        }

        [Fact]
        public void Update_WithEmptyName_ShouldThrowDomainException()
        {
            // Arrange
            var category = CategoryBuilder.Default().Build();

            // Act
            var act = () => category.Update(string.Empty);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage(CategoryErrors.InvalidName.Description);
        }

        [Fact]
        public void Update_WithWhitespaceName_ShouldThrowDomainException()
        {
            // Arrange
            var category = CategoryBuilder.Default().Build();

            // Act
            var act = () => category.Update("   ");

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage(CategoryErrors.InvalidName.Description);
        }

        [Fact]
        public void Update_ShouldMarkAsUpdated()
        {
            // Arrange
            var category = CategoryBuilder.Default().Build();
            var createdAt = category.CreatedAt;

            // Act
            category.Update("New Name");

            // Assert
            category.UpdatedAt.Should().NotBeNull();
            category.UpdatedAt.Should().BeOnOrAfter(createdAt);
        }

        [Fact]
        public void Update_MultipleTimes_ShouldUpdateTimestampEachTime()
        {
            // Arrange
            var category = CategoryBuilder.Default().Build();

            // Act
            category.Update("Name 1");
            var firstUpdate = category.UpdatedAt;
            category.Update("Name 2");
            var secondUpdate = category.UpdatedAt;

            // Assert
            firstUpdate.Should().NotBeNull();
            secondUpdate.Should().NotBeNull();
            secondUpdate.Should().BeOnOrAfter(firstUpdate!.Value);
        }

        #endregion
    }
}
