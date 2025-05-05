using Core.Domain.Entities;
using Core.Domain.Exceptions;

namespace Nutrition.Domain.Entities
{
    public class Diary : BaseEntity<int>
    {
        public int UserId { get; private set; }
        public DateOnly Date { get; private set; }
        public int Calories { get; private set; }

        private readonly List<Meal> _meals = new();
        public IReadOnlyCollection<Meal> Meals => _meals.AsReadOnly();

        private readonly List<Liquid> _liquids = new();
        public IReadOnlyCollection<Liquid> Liquids => _liquids.AsReadOnly();

        public int TotalCalories => CalculateTotalCalories();
        public int TotalLiquidsMl => CalculateTotalLiquids();

        protected Diary()
        { }

        public Diary(int userId, DateOnly date)
        {
            if (userId <= 0)
                throw new DomainException("UserId must be positive.");

            UserId = userId;
            Date = date;
        }

        public void AddMeal(Meal meal)
        {
            Validate(meal.Id.ToString());
            if (meal == null)
                throw new ArgumentNullException(nameof(meal));

            _meals.Add(meal);
        }

        // Método para remover refeição
        public void RemoveMeal(Meal meal)
        {
            if (meal == null)
                throw new ArgumentNullException(nameof(meal));

            if (!_meals.Remove(meal))
                throw new InvalidOperationException("Meal not found in diary.");
        }

        // Método para adicionar líquido
        public void AddLiquid(Liquid liquid)
        {
            if (liquid == null)
                throw new ArgumentNullException(nameof(liquid));

            _liquids.Add(liquid);
        }

        // Método para remover líquido
        public void RemoveLiquid(Liquid liquid)
        {
            if (liquid == null)
                throw new ArgumentNullException(nameof(liquid));

            if (!_liquids.Remove(liquid))
                throw new InvalidOperationException("Liquid not found in diary.");
        }

        // Cálculo automático das calorias totais
        private int CalculateTotalCalories()
        {
            return _meals.Sum(m => m.TotalCalories);
        }

        // Cálculo automático do total de líquidos em ml
        private int CalculateTotalLiquids()
        {
            return _liquids.Sum(l => l.QuantityMl);
        }

        // Atualizar data (se necessário)
        public void UpdateDate(DateOnly newDate)
        {
            Date = newDate;
        }

        private void Validate(string value)
        {
            if (value == null)
                throw new DomainException($"{nameof(value)} cannot be null.");
        }
    }
}
