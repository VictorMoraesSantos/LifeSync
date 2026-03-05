using FluentValidation;

namespace Nutrition.Application.Features.LiquidType.Commands.Create
{
    public class CreateLiquidTypeCommandValidator : AbstractValidator<CreateLiquidTypeCommand>
    {
        public CreateLiquidTypeCommandValidator()
        {
            RuleFor(command => command.Name)
                .NotEmpty().WithMessage("O nome do tipo de líquido é obrigatório.")
                .MaximumLength(100).WithMessage("O nome do tipo de líquido deve ter no máximo 100 caracteres.");
        }
    }
}
