using BuildingBlocks.Results;
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
        Task<Result<bool>> AddMealFoodAsync(int mealId, CreateMealFoodDTO mealFood, CancellationToken cancellationToken);
        Task<Result<bool>> RemoveMealFoodAsync(int mealId, int foodId, CancellationToken cancellationToken);
    }
}
