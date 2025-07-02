using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.Liquid;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Liquid.Commands.Create
{
    public class CreateLiquidCommandHandler : ICommandHandler<CreateLiquidCommand, CreateLiquidResult>
    {
        private readonly ILiquidService _liquidService;

        public CreateLiquidCommandHandler(ILiquidService liquidService)
        {
            _liquidService = liquidService;
        }

        public async Task<Result<CreateLiquidResult>> Handle(CreateLiquidCommand command, CancellationToken cancellationToken)
        {
            CreateLiquidDTO liquid = new(
                command.DiaryId,
                command.Name,
                command.QuantityMl,
                command.CaloriesPerMl);

            var result = await _liquidService.CreateAsync(liquid, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<CreateLiquidResult>(result.Error!);

            return Result.Success(new CreateLiquidResult(result.Value!));
        }
    }
}
