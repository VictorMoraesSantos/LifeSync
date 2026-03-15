using Nutrition.Domain.Entities;
using Nutrition.Domain.ValueObjects;

namespace Nutrition.UnitTests.Helpers.Builders
{
    public class DailyProgressBuilder
    {
        private int _userId = 1;
        private DateOnly _date = DateOnly.FromDateTime(DateTime.UtcNow);
        private int _caloriesConsumed = 0;
        private int _liquidsConsumedMl = 0;
        private DailyGoal? _goal = null;

        public DailyProgressBuilder WithUserId(int userId) { _userId = userId; return this; }
        public DailyProgressBuilder WithDate(DateOnly date) { _date = date; return this; }
        public DailyProgressBuilder WithCaloriesConsumed(int calories) { _caloriesConsumed = calories; return this; }
        public DailyProgressBuilder WithLiquidsConsumedMl(int liquids) { _liquidsConsumedMl = liquids; return this; }
        public DailyProgressBuilder WithGoal(DailyGoal goal) { _goal = goal; return this; }

        public DailyProgress Build()
        {
            var progress = new DailyProgress(_userId, _date, _caloriesConsumed, _liquidsConsumedMl);
            if (_goal != null)
                progress.SetGoal(_goal);
            return progress;
        }
    }
}
