using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.LiquidType;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.LiquidType.Commands.Create
{
    public class CreateLiquidTypeCommandHandler : ICommandHandler<CreateLiquidTypeCommand, CreateLiquidTypeResult>
    {
        private readonly ILiquidTypeService _liquidTypeService;

        public CreateLiquidTypeCommandHandler(ILiquidTypeService liquidTypeService)
        {
            _liquidTypeService = liquidTypeService;
        }

        public async Task<Result<CreateLiquidTypeResult>> Handle(CreateLiquidTypeCommand command, CancellationToken cancellationToken)
        {
            var liquidType = new CreateLiquidTypeDTO(command.Name);

            var result = await _liquidTypeService.CreateAsync(liquidType, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<CreateLiquidTypeResult>(result.Error!);

            return Result.Success(new CreateLiquidTypeResult(result.Value!));
        }
    }
}
