using Core.Domain.Entities;
using Core.Domain.Exceptions;

namespace Nutrition.Domain.Entities
{
    public class Meal : BaseEntity<int>
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int DiaryId { get; private set; }
        public int TotalCalories => _mealFoods?.Sum(i => i.CaloriesPerUnit * i.Quantity) ?? 0;

        private readonly List<MealFood> _mealFoods = new();
        public IReadOnlyCollection<MealFood> MealFoods => _mealFoods.AsReadOnly();

        public Meal(int diaryId ,string name, string description)
        {
            Validate(name);
            Validate(description);
            Name = name;
            Description = description;
            DiaryId = diaryId;
        }

        public void UpdateName(string name)
        {
            Validate(name);
            Name = name;
        }

        public void UpdateDescription(string description)
        {
            Validate(description);
            Description = description;
        }

        public void AddMealFood(MealFood mealFood)
        {
            if (mealFood == null)
                throw new DomainException("MealFood cannot be null");
            _mealFoods.Add(mealFood);
        }

        public void RemoveMealFood(MealFood mealFood)
        {
            if (mealFood == null)
                throw new DomainException("MealFood cannot be null");

            if (!_mealFoods.Remove(mealFood))
                throw new InvalidOperationException("MealFood not found in meal.");
        }

        public void Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException($"{nameof(value)} cannot be empty.");
        }
    }
}
