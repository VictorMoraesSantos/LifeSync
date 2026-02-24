using FluentValidation;

namespace Nutrition.Application.Features.Liquid.Commands.Create
{
    public class CreateLiquidCommandValidator : AbstractValidator<CreateLiquidCommand>
    {
        public CreateLiquidCommandValidator()
        {
            RuleFor(command => command.DiaryId)
                .GreaterThan(0).WithMessage("O ID do diário deve ser maior que zero.");

            RuleFor(command => command.LiquidTypeId)
                .GreaterThan(0).WithMessage("O ID do tipo de líquido deve ser maior que zero.");

            RuleFor(command => command.Quantity)
                .GreaterThan(0).WithMessage("A quantidade em ml deve ser maior que zero.");
        }
    }
}