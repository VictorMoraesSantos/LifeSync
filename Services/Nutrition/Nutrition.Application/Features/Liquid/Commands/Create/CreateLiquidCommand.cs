using MediatR;

namespace Nutrition.Application.Features.Liquid.Commands.Create
{
    public record CreateLiquidCommand(int DiaryId, string Name, int QuantityMl, int CaloriesPerMl) : IRequest<CreateLiquidResult>;
    public record CreateLiquidResult(int Id);
}
