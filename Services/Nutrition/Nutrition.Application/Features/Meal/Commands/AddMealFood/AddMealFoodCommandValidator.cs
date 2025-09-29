using FluentValidation;

namespace Nutrition.Application.Features.Meal.Commands.AddMealFood
{
    public class AddMealFoodCommandValidator : AbstractValidator<AddMealFoodCommand>
    {
        public AddMealFoodCommandValidator()
        {
            RuleFor(command => command.MealFood)
                .NotNull().WithMessage("Os dados do alimento da refeição são obrigatórios.");
        }
    }
}
