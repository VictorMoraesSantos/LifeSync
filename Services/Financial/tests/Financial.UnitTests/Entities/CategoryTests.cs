using Financial.Domain.Entities;

namespace Financial.UnitTests.Entities
{
    public class CategoryTests
    {
        [Fact(DisplayName = "Dado dados válidos, Quando criar categoria, Então deve criar corretamente")]
        public void Deve_Criar_Categoria_Com_Dados_Validos()
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

        [Fact(DisplayName = "Dado dados válidos sem descrição, Quando criar categoria, Então deve criar com descrição nula")]
        public void Deve_Criar_Categoria_Sem_Descricao()
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

        [Theory(DisplayName = "Dado userId inválido, Quando criar categoria, Então deve lançar ArgumentOutOfRangeException")]
        [InlineData(0)]
        [InlineData(-1)]
        public void Deve_Lancar_ArgumentOutOfRangeException_Para_UserId_Invalido(int invalidUserId)
        {
            // Arrange
            string name = "Teste";

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new Category(invalidUserId, name));
        }

        [Fact(DisplayName = "Dado name nulo, Quando criar categoria, Então deve lançar ArgumentNullException")]
        public void Deve_Lancar_ArgumentNullException_Para_Name_Nulo()
        {
            // Arrange
            int userId = 1;
            string? invalidName = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Category(userId, invalidName!));
        }

        [Fact(DisplayName = "Dado name vazio, Quando criar categoria, Então deve lançar ArgumentNullException")]
        public void Deve_Lancar_ArgumentNullException_Para_Name_Vazio()
        {
            // Arrange
            int userId = 1;
            string invalidName = "";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Category(userId, invalidName));
        }

        [Fact(DisplayName = "Dado name com espaço em branco, Quando criar categoria, Então deve lançar ArgumentNullException")]
        public void Deve_Lancar_ArgumentNullException_Para_Name_Whitespace()
        {
            // Arrange
            int userId = 1;
            string invalidName = " ";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Category(userId, invalidName));
        }

        [Fact(DisplayName = "Dado dados válidos, Quando atualizar categoria, Então deve atualizar nome e descrição")]
        public void Deve_Atualizar_Categoria_Com_Dados_Validos()
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

        [Fact(DisplayName = "Dado descrição nula, Quando atualizar categoria, Então deve permitir descrição nula")]
        public void Deve_Atualizar_Categoria_Com_Descricao_Nula()
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

        [Fact(DisplayName = "Dado name nulo, Quando atualizar categoria, Então deve lançar ArgumentNullException")]
        public void Deve_Lancar_ArgumentNullException_Para_Name_Nulo_No_Update()
        {
            // Arrange
            var category = new Category(1, "Categoria", "Descrição");
            string? invalidName = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => category.Update(invalidName!, "Nova Descrição"));
        }

        [Fact(DisplayName = "Dado name vazio, Quando atualizar categoria, Então deve lançar ArgumentNullException")]
        public void Deve_Lancar_ArgumentNullException_Para_Name_Vazio_No_Update()
        {
            // Arrange
            var category = new Category(1, "Categoria", "Descrição");
            string invalidName = "";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => category.Update(invalidName, "Nova Descrição"));
        }

        [Fact(DisplayName = "Dado name com espaço em branco, Quando atualizar categoria, Então deve lançar ArgumentNullException")]
        public void Deve_Lancar_ArgumentNullException_Para_Name_Whitespace_No_Update()
        {
            // Arrange
            var category = new Category(1, "Categoria", "Descrição");
            string invalidName = " ";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => category.Update(invalidName, "Nova Descrição"));
        }

        [Fact(DisplayName = "Dado múltiplas atualizações, Quando atualizar categoria, Então UpdatedAt deve ser alterado a cada vez")]
        public void Deve_Alterar_UpdatedAt_A_Cada_Atualizacao()
        {
            // Arrange
            var category = new Category(1, "Categoria", "Descrição");
            var initialUpdateTime = category.UpdatedAt;

            // Act
            category.Update("Nome 1", "Descrição 1");
            var firstUpdateTime = category.UpdatedAt;
            category.Update("Nome 2", "Descrição 2");
            var secondUpdateTime = category.UpdatedAt;

            // Assert
            Assert.NotEqual(initialUpdateTime, firstUpdateTime);
            Assert.NotEqual(firstUpdateTime, secondUpdateTime);
        }
    }
}