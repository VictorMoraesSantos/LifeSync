using FluentValidation;

namespace Nutrition.Application.Features.LiquidType.Commands.Update
{
    public class UpdateLiquidTypeCommandValidator : AbstractValidator<UpdateLiquidTypeCommand>
    {
        public UpdateLiquidTypeCommandValidator()
        {
            RuleFor(command => command.Id)
                .GreaterThan(0).WithMessage("O ID do tipo de líquido deve ser maior que zero.");

            RuleFor(command => command.Name)
                .NotEmpty().WithMessage("O nome do tipo de líquido é obrigatório.")
                .MaximumLength(100).WithMessage("O nome do tipo de líquido deve ter no máximo 100 caracteres.");
        }
    }
}
