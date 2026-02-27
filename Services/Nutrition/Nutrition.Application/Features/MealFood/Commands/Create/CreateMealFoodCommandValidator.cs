using FluentValidation;

namespace Nutrition.Application.Features.MealFood.Commands.Create
{
    public class CreateMealFoodCommandValidator : AbstractValidator<CreateMealFoodCommand>
    {
        public CreateMealFoodCommandValidator()
        {
            RuleFor(command => command.MealId)
                .GreaterThan(0).WithMessage("O ID do diário deve ser maior que zero.");

            RuleFor(command => command.FoodId)
                .GreaterThan(0).WithMessage("O ID do alimento deve ser maior que zero.");

            RuleFor(command => command.Quantity)
                .GreaterThan(0).WithMessage("A quantidade em gramas deve ser maior que zero.");
        }
    }
}
