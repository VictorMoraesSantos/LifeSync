using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Nutrition.Domain.Events;

namespace Nutrition.Domain.Entities
{
    public class Diary : BaseEntity<int>
    {
        public int UserId { get; private set; }
        public DateOnly Date { get; private set; }
        public int TotalCalories => _liquids.Sum(l => l.TotalCalories) + _meals.Sum(m => m.TotalCalories);
        public int TotalLiquidsMl => _liquids.Sum(l => l.QuantityMl);

        private readonly List<Meal> _meals = new();
        public IReadOnlyCollection<Meal> Meals => _meals.AsReadOnly();

        private readonly List<Liquid> _liquids = new();
        public IReadOnlyCollection<Liquid> Liquids => _liquids.AsReadOnly();

        protected Diary()
        { }

        public Diary(int userId, DateOnly date)
        {
            if (userId <= 0)
                throw new DomainException("UserId must be positive.");

            Date = date;
            UserId = userId;
        }

        public void AddMeal(Meal meal)
        {
            Validate(meal.Id.ToString());
            if (meal == null)
                throw new DomainException("Meal cannot be null");

            _meals.Add(meal);
            AddDomainEvent(new MealAddedToDiaryEvent(UserId, Date, meal.Id));
        }

        public void RemoveMeal(Meal meal)
        {
            if (meal == null)
                throw new DomainException("Meal cannot be null");

            if (!_meals.Remove(meal))
                throw new DomainException("Meal not found in diary.");
        }

        public void AddLiquid(Liquid liquid)
        {
            if (liquid == null)
                throw new DomainException("Liquid cannot be null");

            _liquids.Add(liquid);
        }

        public void RemoveLiquid(Liquid liquid)
        {
            if (liquid == null)
                throw new DomainException("Liquid cannot be null");

            if (!_liquids.Remove(liquid))
                throw new DomainException("Liquid not found in diary.");
        }

        public void UpdateDate(DateOnly newDate)
        {
            Date = newDate;
            MarkAsUpdated();
        }

        private void Validate(string value)
        {
            if (value == null)
                throw new DomainException($"{nameof(value)} cannot be null.");
        }
    }
}
