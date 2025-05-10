using Core.Application.Interfaces;
using Nutrition.Application.DTOs.MealFoods;

namespace Nutrition.Application.Interfaces
{
    public interface IMealFoodService
        : IReadService<MealFoodDTO, int>,
        ICreateService<CreateMealFoodDTO>,
        IUpdateService<UpdateMealFoodDTO>,
        IDeleteService<int>
    {
    }
}
