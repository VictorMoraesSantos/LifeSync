using Core.Domain.Entities;

namespace Nutrition.Domain.Entities
{
    public class Meal : BaseEntity<int>
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int DiaryId { get; private set; } // ID do diário ao qual esta refeição pertence

        private readonly List<MealFood> _mealFoods = new();
        public IReadOnlyCollection<MealFood> MealFoods => _mealFoods.AsReadOnly();

        public int TotalCalories => CalculateTotalCalories();

        public Meal(string name, string description)
        {
            SetName(name);
            SetDescription(description);
        }

        // Atualiza o nome da refeição com validação
        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.", nameof(name));
            Name = name.Trim();
        }

        // Atualiza a descrição da refeição
        public void SetDescription(string description)
        {
            Description = description?.Trim() ?? string.Empty;
        }

        // Adiciona um ingrediente à refeição
        public void AddMealFood(MealFood ingredient)
        {
            if (ingredient == null)
                throw new ArgumentNullException(nameof(ingredient));

            _mealFoods.Add(ingredient);
        }

        // Remove um ingrediente da refeição
        public void RemoveMealFood(MealFood ingredient)
        {
            if (ingredient == null)
                throw new ArgumentNullException(nameof(ingredient));

            if (!_mealFoods.Remove(ingredient))
                throw new InvalidOperationException("MealFood not found in meal.");
        }

        // Atualiza um ingrediente existente (por exemplo, quantidade)
        public void UpdateMealFood(MealFood oldMealFood, MealFood newMealFood)
        {
            if (oldMealFood == null)
                throw new ArgumentNullException(nameof(oldMealFood));
            if (newMealFood == null)
                throw new ArgumentNullException(nameof(newMealFood));

            var index = _mealFoods.IndexOf(oldMealFood);
            if (index == -1)
                throw new InvalidOperationException("MealFood to update not found.");

            _mealFoods[index] = newMealFood;
        }

        // Calcula as calorias totais somando as calorias de cada ingrediente
        private int CalculateTotalCalories()
        {
            return _mealFoods.Sum(i => i.CaloriesPerUnit*i.Quantity);
        }
    }
}
