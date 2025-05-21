using Core.Application.Interfaces;
using Nutrition.Application.DTOs.Meal;

namespace Nutrition.Application.Interfaces
{
    public interface IMealService
        : IReadService<MealDTO, int>,
        ICreateService<CreateMealDTO>,
        IUpdateService<UpdateMealDTO>,
        IDeleteService<int>
    {
    }
}
