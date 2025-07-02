using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Liquid.Commands.Delete
{
    public record DeleteLiquidCommandHandler : ICommandHandler<DeleteLiquidCommand, DeleteLiquidResult>
    {
        private readonly ILiquidService _liquidService;

        public DeleteLiquidCommandHandler(ILiquidService liquidService)
        {
            _liquidService = liquidService;
        }

        public async Task<Result<DeleteLiquidResult>> Handle(DeleteLiquidCommand command, CancellationToken cancellationToken)
        {
            var result = await _liquidService.DeleteAsync(command.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<DeleteLiquidResult>(result.Error!);

            return Result.Success(new DeleteLiquidResult(result.Value!));
        }
    }
}
