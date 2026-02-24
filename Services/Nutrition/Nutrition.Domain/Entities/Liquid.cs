using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Nutrition.Domain.Errors;

namespace Nutrition.Domain.Entities
{
    public class Liquid : BaseEntity<int>
    {
        public int DiaryId { get; private set; }
        public int LiquidTypeId { get; set; }
        public LiquidType LiquidType { get; set; }
        public int Quantity { get; private set; }

        public Liquid(int diaryId, int liquidTypeId, int quantity)
        {
            SetDiaryId(diaryId);
            SetLiquidTypeId(liquidTypeId);
            SetQuantity(quantity);
        }

        public void Update(int liquidTypeId, int quantity)
        {
            SetLiquidTypeId(liquidTypeId);
            SetQuantity(quantity);
            MarkAsUpdated();
        }

        public void SetDiaryId(int diaryId)
        {
            if (diaryId <= 0)
                throw new DomainException(LiquidErrors.InvalidDiaryId);
            DiaryId = diaryId;
        }

        public void SetLiquidTypeId(int liquidTypeId)
        {
            if (liquidTypeId <= 0)
                throw new DomainException(LiquidErrors.InvalidLiquidTypeId);
            LiquidTypeId = liquidTypeId;
        }

        public void SetQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException(LiquidErrors.InvalidQuantity);
            Quantity = quantity;
        }
    }
}