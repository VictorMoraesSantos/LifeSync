using FluentValidation;
using TaskManager.Application.Features.TaskLabels.Commands.Update;

namespace TaskManager.Application.Features.TaskLabels.Commands.Create
{
    public class UpdateTaskLabelCommandValidator : AbstractValidator<UpdateTaskLabelCommand>
    {
        public UpdateTaskLabelCommandValidator()
        {
            RuleFor(command => command.Name)
                .NotEmpty().WithMessage("O nome da etiqueta é obrigatório.")
                .MinimumLength(2).WithMessage("O nome da etiqueta deve ter no mínimo 2 caracteres.")
                .MaximumLength(50).WithMessage("O nome da etiqueta deve ter no máximo 50 caracteres.");

            RuleFor(command => command.LabelColor)
                .IsInEnum().WithMessage("A cor da etiqueta informada é inválida.");
        }
    }
}