using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.LiquidType.Commands.Delete
{
    public record DeleteLiquidTypeCommandHandler : ICommandHandler<DeleteLiquidTypeCommand, DeleteLiquidTypeResult>
    {
        private readonly ILiquidTypeService _liquidTypeService;

        public DeleteLiquidTypeCommandHandler(ILiquidTypeService liquidTypeService)
        {
            _liquidTypeService = liquidTypeService;
        }

        public async Task<Result<DeleteLiquidTypeResult>> Handle(DeleteLiquidTypeCommand command, CancellationToken cancellationToken)
        {
            var result = await _liquidTypeService.DeleteAsync(command.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<DeleteLiquidTypeResult>(result.Error!);

            return Result.Success(new DeleteLiquidTypeResult(result.Value!));
        }
    }
}
