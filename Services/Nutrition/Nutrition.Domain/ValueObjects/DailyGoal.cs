using Core.Domain.Exceptions;

namespace Nutrition.Domain.ValueObjects
{
    public class DailyGoal
    {
        public int Calories { get; private set; } = 0;
        public int QuantityMl { get; private set; } = 0;

        public DailyGoal(int calories, int quantityMl)
        {
            Validate(calories);
            Validate(quantityMl);
            Calories = calories;
            QuantityMl = quantityMl;
        }

        private void Validate(int value)
        {
            if (value < 0)
                throw new DomainException($"{nameof(value)} cannot be negative.");
        }
    }
}
