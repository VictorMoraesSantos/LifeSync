using MediatR;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.Liquid.Commands.Delete
{
    public record DeleteLiquidCommandHandler : IRequestHandler<DeleteLiquidCommand, DeleteLiquidResult>
    {
        private readonly ILiquidService _liquidService;

        public DeleteLiquidCommandHandler(ILiquidService liquidService)
        {
            _liquidService = liquidService;
        }

        public async Task<DeleteLiquidResult> Handle(DeleteLiquidCommand command, CancellationToken cancellationToken)
        {
            bool result = await _liquidService.DeleteAsync(command.Id, cancellationToken);
            DeleteLiquidResult response = new(result);
            return response;
        }
    }
}
