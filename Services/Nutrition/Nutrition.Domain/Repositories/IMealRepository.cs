using Core.Domain.Repositories;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Filters;

namespace Nutrition.Domain.Repositories
{
    public interface IMealRepository : IRepository<Meal, int, MealQueryFilter>
    {

    }
}
