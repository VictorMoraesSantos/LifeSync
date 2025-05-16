using MediatR;
using Nutrition.Application.DTOs.Liquid;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.Liquid.Commands.Create
{
    public class CreateLiquidCommandHandler : IRequestHandler<CreateLiquidCommand, CreateLiquidResult>
    {
        private readonly ILiquidService _liquidService;

        public CreateLiquidCommandHandler(ILiquidService liquidService)
        {
            _liquidService = liquidService;
        }

        public async Task<CreateLiquidResult> Handle(CreateLiquidCommand command, CancellationToken cancellationToken)
        {
            CreateLiquidDTO liquid = new(
                command.DiaryId,
                command.Name,
                command.QuantityMl,
                command.CaloriesPerMl);

            bool result = await _liquidService.CreateAsync(liquid, cancellationToken);
            CreateLiquidResult response = new(result);
            return response;
        }
    }
}
