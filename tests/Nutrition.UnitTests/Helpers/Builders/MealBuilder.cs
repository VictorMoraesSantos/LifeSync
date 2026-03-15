using Nutrition.Domain.Entities;

namespace Nutrition.UnitTests.Helpers.Builders
{
    public class MealBuilder
    {
        private string _name = "Almoço";
        private string _description = "Refeição do dia";

        public MealBuilder WithName(string name) { _name = name; return this; }
        public MealBuilder WithDescription(string description) { _description = description; return this; }

        public Meal Build()
        {
            return new Meal(_name, _description);
        }
    }
}
