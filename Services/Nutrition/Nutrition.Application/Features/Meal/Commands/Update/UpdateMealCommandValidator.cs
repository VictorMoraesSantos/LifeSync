using FluentValidation;
using Nutrition.Application.Features.Meal.Commands.Update;

namespace Nutrition.Application.Features.Meal.Commands.Create
{
    public class UpdateMealCommandValidator : AbstractValidator<UpdateMealCommand>
    {
        public UpdateMealCommandValidator()
        {
            RuleFor(command => command.Name)
                .NotEmpty().WithMessage("O nome da refeição é obrigatório.")
                .MaximumLength(100).WithMessage("O nome da refeição deve ter no máximo 100 caracteres.");

            RuleFor(command => command.Description)
                .NotEmpty().WithMessage("A descrição da refeição é obrigatória.")
                .MaximumLength(500).WithMessage("A descrição da refeição deve ter no máximo 500 caracteres.");
        }
    }
}