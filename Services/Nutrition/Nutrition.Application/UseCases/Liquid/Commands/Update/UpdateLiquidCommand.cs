using MediatR;

namespace Nutrition.Application.UseCases.Liquid.Commands.Update
{
    public record UpdateLiquidCommand(int Id, string Name, int QuantityMl, int CaloriesPerMl) : IRequest<UpdateLiquidResult>;
    public record UpdateLiquidResult(bool IsSuccess);
}
