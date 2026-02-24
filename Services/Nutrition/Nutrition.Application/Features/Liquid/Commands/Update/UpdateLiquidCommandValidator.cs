using FluentValidation;
using Nutrition.Application.Features.Liquid.Commands.Update;

namespace Nutrition.Application.Features.Liquid.Commands.Create
{
    public class UpdateLiquidCommandValidator : AbstractValidator<UpdateLiquidCommand>
    {
        public UpdateLiquidCommandValidator()
        {
            RuleFor(command => command.Id)
                .GreaterThan(0).WithMessage("O ID do líquido deve ser maior que zero.");

            RuleFor(command => command.LiquidTypeId)
                .GreaterThan(0).WithMessage("O ID do tipo de líquido deve ser maior que zero.");

            RuleFor(command => command.Quantity)
                .GreaterThan(0).WithMessage("A quantidade em ml deve ser maior que zero.");
        }
    }
}