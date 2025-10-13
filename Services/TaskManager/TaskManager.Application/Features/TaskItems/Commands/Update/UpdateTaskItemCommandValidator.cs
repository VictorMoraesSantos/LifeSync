﻿using FluentValidation;

namespace TaskManager.Application.Features.TaskItems.Commands.Update
{
    public class UpdateTaskItemCommandValidator : AbstractValidator<UpdateTaskItemCommand>
    {
        public UpdateTaskItemCommandValidator()
        {
            RuleFor(command => command.Title)
                .NotEmpty().WithMessage("O título é obrigatório.")
                .MinimumLength(3).WithMessage("O título deve ter no mínimo 3 caracteres.")
                .MaximumLength(100).WithMessage("O título deve ter no máximo 100 caracteres.");

            RuleFor(command => command.Description)
                .NotEmpty().WithMessage("A descrição é obrigatória.")
                .MinimumLength(5).WithMessage("a descrição deve ter no mínimo 5 caracteres.")
                .MaximumLength(500).WithMessage("a descrição deve ter no máximo 500 caracteres.");

            RuleFor(command => command.Priority)
                .IsInEnum().WithMessage("A prioridade informada é inválida.");

            RuleFor(command => command.DueDate)
                .NotEmpty().WithMessage("A data de vencimento é obrigatória.")
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("A data de vencimento não pode ser anterior à data atual.");
        }
    }
}
