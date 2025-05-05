namespace Nutrition.Domain.Entities
{
    public class Liquid
    {
        public int Id { get; private set; }  // Opcional, caso queira controlar Id
        public string Name { get; private set; }
        public int QuantityMl { get; private set; }
        public int CaloriesPerMl { get; private set; }

        public int TotalCalories => CalculateTotalCalories();

        public Liquid(string name, int quantityMl, int caloriesPerMl = 0)
        {
            SetName(name);
            SetQuantityMl(quantityMl);
            SetCaloriesPerMl(caloriesPerMl);
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

        private int CalculateTotalCalories()
        {
            return QuantityMl * CaloriesPerMl;
        }
    }
}