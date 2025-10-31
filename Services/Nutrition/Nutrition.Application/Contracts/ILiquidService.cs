using Core.Application.Interfaces;
using Nutrition.Application.DTOs.Liquid;

namespace Nutrition.Application.Interfaces
{
    public interface ILiquidService
        : IReadService<LiquidDTO, int, LiquidQueryFilterDTO>,
        ICreateService<CreateLiquidDTO>,
        IUpdateService<UpdateLiquidDTO>,
        IDeleteService<int>
    {
    }
}
