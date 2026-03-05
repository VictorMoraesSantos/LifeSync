using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.LiquidType;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.LiquidType.Commands.Update
{
    public class UpdateLiquidTypeCommandHandler : ICommandHandler<UpdateLiquidTypeCommand, UpdateLiquidTypeResult>
    {
        private readonly ILiquidTypeService _liquidTypeService;

        public UpdateLiquidTypeCommandHandler(ILiquidTypeService liquidTypeService)
        {
            _liquidTypeService = liquidTypeService;
        }

        public async Task<Result<UpdateLiquidTypeResult>> Handle(UpdateLiquidTypeCommand command, CancellationToken cancellationToken)
        {
            var dto = new UpdateLiquidTypeDTO(
                command.Id,
                command.Name);

            var result = await _liquidTypeService.UpdateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<UpdateLiquidTypeResult>(result.Error!);

            return Result.Success(new UpdateLiquidTypeResult(result.Value!));
        }
    }
}
