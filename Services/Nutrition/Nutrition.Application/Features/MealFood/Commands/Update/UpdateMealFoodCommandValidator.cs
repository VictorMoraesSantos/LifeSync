using FluentValidation;

namespace Nutrition.Application.Features.MealFood.Commands.Update
{
    public class UpdateMealFoodCommandValidator : AbstractValidator<UpdateMealFoodCommand>
    {
        public UpdateMealFoodCommandValidator()
        {
            RuleFor(command => command.Id)
                .GreaterThan(0).WithMessage("O ID do alimento deve ser maior que zero");

            RuleFor(command => command.FoodId)
                .GreaterThan(0).WithMessage("O ID do alimento deve ser maior que zero.");

            RuleFor(command => command.Quantity)
                .GreaterThan(0).WithMessage("A quantidade em gramas deve ser maior que zero.");

        }
    }
}
