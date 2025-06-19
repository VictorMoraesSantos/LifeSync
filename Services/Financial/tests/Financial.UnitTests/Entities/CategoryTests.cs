using Financial.Domain.Entities;

namespace Financial.UnitTests.Entities
{
    public class CategoryTests
    {
        [Fact]
        public void Category_ValidInput_CreatesCategory()
        {
            // Arrange
            int userId = 1;
            string name = "Alimentação";
            string? description = "Gastos com comida";

            // Act
            var category = new Category(userId, name, description);

            // Assert
            Assert.Equal(userId, category.UserId);
            Assert.Equal(name, category.Name);
            Assert.Equal(description, category.Description);
        }

        [Fact]
        public void Category_ValidInputWithoutDescription_CreatesCategory()
        {
            // Arrange
            int userId = 1;
            string name = "Lazer";

            // Act
            var category = new Category(userId, name);

            // Assert
            Assert.Equal(userId, category.UserId);
            Assert.Equal(name, category.Name);
            Assert.Null(category.Description);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Category_InvalidUserId_ThrowsException(int invalidUserId)
        {
            // Arrange
            string name = "Teste";

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new Category(invalidUserId, name));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Category_InvalidName_ThrowsException(string invalidName)
        {
            // Arrange
            int userId = 1;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Category(userId, invalidName));
        }

        [Fact]
        public void Update_ValidInput_UpdatesNameAndDescription()
        {
            // Arrange
            var category = new Category(1, "Categoria Antiga", "Descrição Antiga");
            string newName = "Categoria Nova";
            string newDescription = "Descrição Nova";
            DateTime creationTime = category.CreatedAt;

            // Act
            category.Update(newName, newDescription);

            // Assert
            Assert.Equal(newName, category.Name);
            Assert.Equal(newDescription, category.Description);
            Assert.NotEqual(creationTime, category.UpdatedAt);
        }

        [Fact]
        public void Update_ValidInputNullDescription_UpdatesNameAndSetsDescriptionToNull()
        {
            // Arrange
            var category = new Category(1, "Categoria Antiga", "Descrição Antiga");
            string newName = "Categoria Nova";
            string? newDescription = null;
            DateTime creationTime = category.CreatedAt;

            // Act
            category.Update(newName, newDescription);

            // Assert
            Assert.Equal(newName, category.Name);
            Assert.Null(category.Description);
            Assert.NotEqual(creationTime, category.UpdatedAt);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Update_InvalidName_ThrowsException(string invalidName)
        {
            // Arrange
            var category = new Category(1, "Categoria", "Descrição");
            string? description = "Nova Descrição";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => category.Update(invalidName, description));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Update_InvalidDescription_ThrowsException(string nullDescription)
        {
            // Arrange
            var name = "Nova Categoria";
            var description = "Descrição";
            var category = new Category(1, name, description);
            var updatedName = "Nova Categoria";

            // Act & Assert
            category.Update(updatedName, nullDescription);

            Assert.Equal(1, category.UserId);
            Assert.Equal(updatedName, category.Name);
            Assert.Equal(nullDescription, category.Description);
        }
    }
}
