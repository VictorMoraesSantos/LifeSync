using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.Features.TaskItems.Commands.AddLabel;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Commands.RemoveLabel
{
    public class RemoveLabelCommandHandler : SecureCommandHandlerBase, ICommandHandler<RemoveLabelCommand, RemoveLabelResult>
    {
        private readonly ITaskItemService _taskItemService;
        private readonly IValidator<RemoveLabelCommand> _validator;

        public RemoveLabelCommandHandler(
            IValidator<RemoveLabelCommand> validator,
            IHttpContextAccessor httpContext) : base(httpContext)
        {
            _validator = validator;
        }

        public async Task<Result<RemoveLabelResult>> Handle(RemoveLabelCommand command, CancellationToken cancellationToken)
        {
            ValidationResult validation = await _validator.ValidateAsync(command, cancellationToken);
            if (!validation.IsValid)
            {
                List<string> errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return Result.Failure<RemoveLabelResult>(new Error(string.Join("; ", errors), ErrorType.Validation));
            }

            Result<TaskItemDTO?> existingTask = await _taskItemService.GetByIdAsync(command.TaskItemId, cancellationToken);
            if (!existingTask.IsSuccess)
                return Result.Failure<RemoveLabelResult>(existingTask.Error!);

            Result<RemoveLabelResult> accessValidation = ValidateAccess<RemoveLabelResult>(existingTask.Value!.UserId);
            if (!accessValidation.IsSuccess)
                return Result.Failure<RemoveLabelResult>(accessValidation.Error!);

            var dto = new UpdateLabelsDTO(command.TaskItemId, command.TaskLabelsId);

            Result<bool> result = await _taskItemService.RemoveLabelAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<RemoveLabelResult>(result.Error!);

            return Result.Success<RemoveLabelResult>(new RemoveLabelResult(result.Value));
        }
    }
}
