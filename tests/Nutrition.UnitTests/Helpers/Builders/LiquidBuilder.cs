using Nutrition.Domain.Entities;

namespace Nutrition.UnitTests.Helpers.Builders
{
    public class LiquidBuilder
    {
        private int _diaryId = 1;
        private int _liquidTypeId = 1;
        private int _quantity = 250;

        public LiquidBuilder WithDiaryId(int diaryId) { _diaryId = diaryId; return this; }
        public LiquidBuilder WithLiquidTypeId(int liquidTypeId) { _liquidTypeId = liquidTypeId; return this; }
        public LiquidBuilder WithQuantity(int quantity) { _quantity = quantity; return this; }

        public Liquid Build()
        {
            return new Liquid(_diaryId, _liquidTypeId, _quantity);
        }
    }
}
