using Core.Application.Interfaces;
using Nutrition.Application.DTOs.Meals;

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
