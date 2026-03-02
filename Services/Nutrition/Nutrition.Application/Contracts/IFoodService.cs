using Core.Application.Interfaces;
using Nutrition.Application.DTOs.Food;

namespace Nutrition.Application.Contracts
{
    public interface IFoodService
        : IReadService<FoodDTO, int, FoodQueryFilterDTO>,
        IDeleteService<int>
    {
    }
}
