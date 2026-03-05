using Core.Application.Interfaces;
using Nutrition.Application.DTOs.LiquidType;

namespace Nutrition.Application.Interfaces
{
    public interface ILiquidTypeService
        : IReadService<LiquidTypeDTO, int, LiquidTypeQueryFilterDTO>,
        ICreateService<CreateLiquidTypeDTO>,
        IUpdateService<UpdateLiquidTypeDTO>,
        IDeleteService<int>
    {
    }
}
