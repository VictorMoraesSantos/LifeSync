using FluentValidation;

namespace Nutrition.Application.Features.MealFood.Commands.Create
{
    public class CreateMealFoodCommandValidator : AbstractValidator<CreateMealFoodCommand>
    {
        public CreateMealFoodCommandValidator()
        {
            RuleFor(command => command.Name)
                .NotEmpty().WithMessage("O nome do alimento é obrigatório.")
                .MaximumLength(100).WithMessage("O nome do alimento deve ter no máximo 100 caracteres.");

            RuleFor(command => command.Quantity)
                .GreaterThan(0).WithMessage("A quantidade em gramas deve ser maior que zero.");

            RuleFor(command => command.Calories)
                .GreaterThanOrEqualTo(0).WithMessage("As calorias por unidade não podem ser negativas.");
        }
    }
}
