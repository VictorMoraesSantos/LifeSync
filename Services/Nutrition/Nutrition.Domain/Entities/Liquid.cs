using Core.Domain.Entities;

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
                throw new ArgumentException("Name cannot be empty.", nameof(name));
            Name = name.Trim();
        }

        public void SetQuantityMl(int quantityMl)
        {
            if (quantityMl <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantityMl), "Quantity must be positive.");
            QuantityMl = quantityMl;
        }

        public void SetCaloriesPerMl(int caloriesPerMl)
        {
            if (caloriesPerMl < 0)
                throw new ArgumentOutOfRangeException(nameof(caloriesPerMl), "Calories per ml cannot be negative.");
            CaloriesPerMl = caloriesPerMl;
        }
    }
}