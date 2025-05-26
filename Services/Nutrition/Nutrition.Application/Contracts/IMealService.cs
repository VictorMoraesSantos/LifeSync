using Core.Application.Interfaces;
using Nutrition.Application.DTOs.Meal;
using Nutrition.Application.DTOs.MealFood;

namespace Nutrition.Application.Interfaces
{
    public interface IMealService
        : IReadService<MealDTO, int>,
        ICreateService<CreateMealDTO>,
        IUpdateService<UpdateMealDTO>,
        IDeleteService<int>
    {
        Task<bool> AddMealFoodAsync(int mealId, CreateMealFoodDTO mealFood, CancellationToken cancellationToken);
    }
}
