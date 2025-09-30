using FluentValidation;

namespace Financial.Application.Features.Categories.Commands.Create
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(command => command.Name)
                .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
                .MinimumLength(2).WithMessage("O nome da categoria deve ter no mínimo 2 caracteres.")
                .MaximumLength(50).WithMessage("O nome da categoria deve ter no máximo 50 caracteres.");

            RuleFor(command => command.Description)
                .MaximumLength(200).WithMessage("A descrição da categoria deve ter no máximo 200 caracteres.")
                .When(command => !string.IsNullOrEmpty(command.Description));
        }
    }
}