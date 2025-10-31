using Core.Application.Interfaces;
using Nutrition.Application.DTOs.MealFood;

namespace Nutrition.Application.Interfaces
{
    public interface IMealFoodService
        : IReadService<MealFoodDTO, int, MealFoodQueryFilterDTO>,
        ICreateService<CreateMealFoodDTO>,
        IUpdateService<UpdateMealFoodDTO>,
        IDeleteService<int>
    {
    }
}
