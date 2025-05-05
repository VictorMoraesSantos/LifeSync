using Core.Domain.Entities;

namespace Nutrition.Domain.Entities
{
    public class MealFood : BaseEntity<int>
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Quantity { get; private set; } // Quantidade em gramas ou ml
        public int CaloriesPerUnit { get; private set; } // Calorias por grama ou ml
        public int MealId { get; private set; } // ID da refeição à qual este ingrediente pertence

        public int TotalCalories => CalculateTotalCalories();

        public MealFood(string name, int quantity, int caloriesPerUnit, string description = "")
        {
            SetName(name);
            SetQuantity(quantity);
            SetCaloriesPerUnit(caloriesPerUnit);
            SetDescription(description);
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.", nameof(name));
            Name = name.Trim();
        }

        public void SetDescription(string description)
        {
            Description = description?.Trim() ?? string.Empty;
        }

        public void SetQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");
            Quantity = quantity;
        }

        public void SetCaloriesPerUnit(int caloriesPerUnit)
        {
            if (caloriesPerUnit < 0)
                throw new ArgumentOutOfRangeException(nameof(caloriesPerUnit), "Calories per unit cannot be negative.");
            CaloriesPerUnit = caloriesPerUnit;
        }

        private int CalculateTotalCalories()
        {
            return Quantity * CaloriesPerUnit;
        }
    }
}
