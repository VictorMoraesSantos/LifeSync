using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Nutrition.Domain.Events;

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

        public Meal(string name, string description)
        {
            Validate(name);
            Validate(description);

            Name = name;
            Description = description;
        }

        public void SetDiaryId(int diaryId)
        {
            if (diaryId <= 0)
                throw new DomainException("DiaryId must be positive.");

            DiaryId = diaryId;
        }

        public void Update(string name, string description)
        {
            Validate(name);
            Validate(description);

            Name = name;
            Description = description;
        }

        public void AddMealFood(MealFood mealFood)
        {
            if (mealFood == null)
                throw new DomainException("MealFood cannot be null");

            _mealFoods.Add(mealFood);

            AddDomainEvent(new MealFoodAddedEvent(DiaryId, mealFood.TotalCalories));
        }

        public void RemoveMealFood(int mealFoodId)
        {
            if (mealFoodId == null)
                throw new DomainException("MealFood cannot be null");

            var mealFood = _mealFoods.FirstOrDefault(mf => mf.Id == mealFoodId);

            _mealFoods.Remove(mealFood);

            AddDomainEvent(new MealFoodRemovedEvent(DiaryId, mealFood.TotalCalories));
        }

        public void Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException($"{nameof(value)} cannot be empty.");
        }
    }
}
