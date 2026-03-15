using Core.Domain.Exceptions;
using FluentAssertions;
using Nutrition.Domain.Entities;
using Nutrition.UnitTests.Helpers.Builders;

namespace Nutrition.UnitTests.Domain.Entities
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Domain")]
    public class LiquidTests
    {
        [Fact]
        public void Constructor_ValidParameters_ShouldCreateLiquid()
        {
            // Arrange
            var diaryId = 1;
            var liquidTypeId = 2;
            var quantity = 500;

            // Act
            var liquid = new Liquid(diaryId, liquidTypeId, quantity);

            // Assert
            liquid.DiaryId.Should().Be(diaryId);
            liquid.LiquidTypeId.Should().Be(liquidTypeId);
            liquid.Quantity.Should().Be(quantity);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Constructor_InvalidDiaryId_ShouldThrowDomainException(int invalidDiaryId)
        {
            // Act
            var act = () => new Liquid(invalidDiaryId, 1, 250);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_InvalidLiquidTypeId_ShouldThrowDomainException(int invalidLiquidTypeId)
        {
            // Act
            var act = () => new Liquid(1, invalidLiquidTypeId, 250);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-500)]
        public void Constructor_InvalidQuantity_ShouldThrowDomainException(int invalidQuantity)
        {
            // Act
            var act = () => new Liquid(1, 1, invalidQuantity);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Update_ValidParameters_ShouldUpdateLiquidTypeIdAndQuantity()
        {
            // Arrange
            var liquid = new LiquidBuilder().Build();
            var newLiquidTypeId = 3;
            var newQuantity = 750;

            // Act
            liquid.Update(newLiquidTypeId, newQuantity);

            // Assert
            liquid.LiquidTypeId.Should().Be(newLiquidTypeId);
            liquid.Quantity.Should().Be(newQuantity);
            liquid.UpdatedAt.Should().NotBeNull();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Update_InvalidLiquidTypeId_ShouldThrowDomainException(int invalidLiquidTypeId)
        {
            // Arrange
            var liquid = new LiquidBuilder().Build();

            // Act
            var act = () => liquid.Update(invalidLiquidTypeId, 250);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Update_InvalidQuantity_ShouldThrowDomainException(int invalidQuantity)
        {
            // Arrange
            var liquid = new LiquidBuilder().Build();

            // Act
            var act = () => liquid.Update(1, invalidQuantity);

            // Assert
            act.Should().Throw<DomainException>();
        }
    }
}
