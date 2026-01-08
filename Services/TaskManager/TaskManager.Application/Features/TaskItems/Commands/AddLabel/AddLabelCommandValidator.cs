using FluentValidation;

namespace TaskManager.Application.Features.TaskItems.Commands.AddLabel
{
    public class AddLabelCommandValidator : AbstractValidator<AddLabelCommand>
    {
        public AddLabelCommandValidator()
        {
            RuleFor(command => command.TaskItemId)
                .NotEmpty().WithMessage("O ID do item de tarefa é obrigatório.")
                .GreaterThan(0).WithMessage("O ID do item de tarefa deve ser maior que zero.");

            RuleFor(command => command.TaskLabelsId)
                .NotEmpty().WithMessage("O ID do rótulo da tarefa é obrigatório.")
                .Must(list => list.All(id => id > 0))
                .WithMessage("Todos os IDs dos rótulos da tarefa devem ser maiores que zero.");
        }
    }
}
