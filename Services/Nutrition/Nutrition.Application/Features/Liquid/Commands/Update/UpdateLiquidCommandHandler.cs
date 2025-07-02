using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.Liquid;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Liquid.Commands.Update
{
    public class UpdateLiquidCommandHandler : ICommandHandler<UpdateLiquidCommand, UpdateLiquidResult>
    {
        private readonly ILiquidService _liquidService;

        public UpdateLiquidCommandHandler(ILiquidService liquidService)
        {
            _liquidService = liquidService;
        }

        public async Task<Result<UpdateLiquidResult>> Handle(UpdateLiquidCommand command, CancellationToken cancellationToken)
        {
            UpdateLiquidDTO dto = new(
                command.Id,
                command.Name,
                command.QuantityMl,
                command.CaloriesPerMl
            );

            var result = await _liquidService.UpdateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<UpdateLiquidResult>(result.Error!);

            return Result.Success(new UpdateLiquidResult(result.Value!));
        }
    }
}
