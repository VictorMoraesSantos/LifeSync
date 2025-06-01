using BuildingBlocks.CQRS.Request;
using Nutrition.Application.DTOs.Liquid;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Liquid.Commands.Update
{
    public class UpdateLiquidCommandHandler : IRequestHandler<UpdateLiquidCommand, UpdateLiquidResult>
    {
        private readonly ILiquidService _liquidService;

        public UpdateLiquidCommandHandler(ILiquidService liquidService)
        {
            _liquidService = liquidService;
        }

        public async Task<UpdateLiquidResult> Handle(UpdateLiquidCommand command, CancellationToken cancellationToken)
        {
            UpdateLiquidDTO dto = new(
                command.Id,
                command.Name,
                command.QuantityMl,
                command.CaloriesPerMl
            );

            bool result = await _liquidService.UpdateAsync(dto, cancellationToken);

            return new UpdateLiquidResult(result);
        }
    }
}
