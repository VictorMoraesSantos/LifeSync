using Nutrition.Domain.Entities;

namespace Nutrition.UnitTests.Helpers.Builders
{
    public class DiaryBuilder
    {
        private int _userId = 1;
        private DateOnly _date = DateOnly.FromDateTime(DateTime.UtcNow);

        public DiaryBuilder WithUserId(int userId) { _userId = userId; return this; }
        public DiaryBuilder WithDate(DateOnly date) { _date = date; return this; }

        public Diary Build()
        {
            return new Diary(_userId, _date);
        }
    }
}
