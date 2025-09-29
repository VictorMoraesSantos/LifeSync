using FluentValidation;

namespace Nutrition.Application.Features.Liquid.Commands.Create
{
    public class CreateLiquidCommandValidator : AbstractValidator<CreateLiquidCommand>
    {
        public CreateLiquidCommandValidator()
        {
            RuleFor(command => command.Name)
                .NotEmpty().WithMessage("O nome do líquido é obrigatório.")
                .MaximumLength(100).WithMessage("O nome do líquido deve ter no máximo 100 caracteres.");

            RuleFor(command => command.QuantityMl)
                .GreaterThan(0).WithMessage("A quantidade em ml deve ser maior que zero.");

            RuleFor(command => command.CaloriesPerMl)
                .GreaterThanOrEqualTo(0).WithMessage("As calorias por ml não podem ser negativas.");
        }
    }
}