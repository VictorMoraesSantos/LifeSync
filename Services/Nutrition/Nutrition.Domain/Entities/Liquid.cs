using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Nutrition.Domain.Errors;

namespace Nutrition.Domain.Entities
{
    public class Liquid : BaseEntity<int>
    {
        public string Name { get; private set; }
        public int QuantityMl { get; private set; }
        public int CaloriesPerMl { get; private set; } = 0;
        public int DiaryId { get; private set; }
        public int TotalCalories => QuantityMl * CaloriesPerMl;

        public Liquid(int diaryId, string name, int quantityMl, int caloriesPerMl)
        {
            DiaryId = diaryId;
            SetName(name);
            SetQuantityMl(quantityMl);
            SetCaloriesPerMl(caloriesPerMl);
        }

        public void Update(string name, int quantityMl, int caloriesPerMl)
        {
            SetName(name);
            SetQuantityMl(quantityMl);
            SetCaloriesPerMl(caloriesPerMl);
            MarkAsUpdated();
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException(LiquidErrors.InvalidName);
            Name = name.Trim();
        }

        public void SetQuantityMl(int quantityMl)
        {
            if (quantityMl <= 0)
                throw new DomainException(LiquidErrors.InvalidQuantity);
            QuantityMl = quantityMl;
        }

        public void SetCaloriesPerMl(int caloriesPerMl)
        {
            if (caloriesPerMl < 0)
                throw new DomainException(LiquidErrors.NegativeCalories);
            CaloriesPerMl = caloriesPerMl;
        }
    }
}