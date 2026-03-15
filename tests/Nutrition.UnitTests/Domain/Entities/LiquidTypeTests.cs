using Core.Domain.Exceptions;
using FluentAssertions;
using Nutrition.Domain.Entities;

namespace Nutrition.UnitTests.Domain.Entities
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Domain")]
    public class LiquidTypeTests
    {
        [Fact]
        public void Constructor_ValidName_ShouldCreateLiquidType()
        {
            // Arrange
            var name = "Água";

            // Act
            var liquidType = new LiquidType(name);

            // Assert
            liquidType.Name.Should().Be(name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_InvalidName_ShouldThrowDomainException(string? invalidName)
        {
            // Act
            var act = () => new LiquidType(invalidName!);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Update_ValidName_ShouldUpdateName()
        {
            // Arrange
            var liquidType = new LiquidType("Água");
            var newName = "Suco";

            // Act
            liquidType.Update(newName);

            // Assert
            liquidType.Name.Should().Be(newName);
            liquidType.UpdatedAt.Should().NotBeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_InvalidName_ShouldThrowDomainException(string? invalidName)
        {
            // Arrange
            var liquidType = new LiquidType("Água");

            // Act
            var act = () => liquidType.Update(invalidName!);

            // Assert
            act.Should().Throw<DomainException>();
        }
    }
}
