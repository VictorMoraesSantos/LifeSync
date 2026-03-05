using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Nutrition.Domain.Errors;
using Nutrition.Domain.Events;

namespace Nutrition.Domain.Entities
{
    public class Diary : BaseEntity<int>
    {
        public int UserId { get; private set; }
        public DateOnly Date { get; private set; }
        public int TotalCalories => _meals.Sum(m => m.TotalCalories);
        public int TotalLiquidsMl => _liquids.Sum(l => l.Quantity);

        private readonly List<Meal> _meals = new();
        public IReadOnlyCollection<Meal> Meals => _meals.AsReadOnly();

        private readonly List<Liquid> _liquids = new();
        public IReadOnlyCollection<Liquid> Liquids => _liquids.AsReadOnly();

        protected Diary()
        { }

        public Diary(int userId, DateOnly date)
        {
            if (userId <= 0)
                throw new DomainException(DiaryErrors.InvalidUserId);

            Date = date;
            UserId = userId;
        }

        public void AddMeal(Meal meal)
        {
            Validate(meal.Id.ToString());
            if (meal == null)
                throw new DomainException(DiaryErrors.NullMeal);

            _meals.Add(meal);
            AddDomainEvent(new MealAddedToDiaryEvent(UserId, Date, meal.Id));
        }

        public void RemoveMeal(Meal meal)
        {
            if (meal == null)
                throw new DomainException(DiaryErrors.NullMeal);

            if (!_meals.Remove(meal))
                throw new DomainException(DiaryErrors.MealNotFound);
        }

        public void AddLiquid(Liquid liquid)
        {
            if (liquid == null)
                throw new DomainException(DiaryErrors.NullLiquid);

            _liquids.Add(liquid);
            AddDomainEvent(new LiquidChangedEvent(Id));
        }

        public void RemoveLiquid(Liquid liquid)
        {
            if (liquid == null)
                throw new DomainException(DiaryErrors.NullLiquid);

            if (!_liquids.Remove(liquid))
                throw new DomainException(DiaryErrors.LiquidNotFound);

            AddDomainEvent(new LiquidChangedEvent(Id));
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
