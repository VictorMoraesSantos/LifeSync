using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;
using FluentValidation.Results;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Commands.CreateTaskItem
{
    public class CreateTaskItemCommandHandler : ICommandHandler<CreateTaskItemCommand, CreateTaskItemResult>
    {
        private readonly ITaskItemService _taskItemService;
        private readonly IValidator<CreateTaskItemCommand> _validator;

        public CreateTaskItemCommandHandler(ITaskItemService taskItemService, IValidator<CreateTaskItemCommand> validator)
        {
            _taskItemService = taskItemService;
            _validator = validator;
        }

        public async Task<Result<CreateTaskItemResult>> Handle(CreateTaskItemCommand command, CancellationToken cancellationToken)
        {
            ValidationResult validation = await _validator.ValidateAsync(command, cancellationToken);
            if (!validation.IsValid)
            {
                List<string> errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return Result.Failure<CreateTaskItemResult>(new Error(string.Join("; ", errors), ErrorType.Validation));
            }

            CreateTaskItemDTO dto = new(
                command.Title,
                command.Description,
                command.Priority,
                command.DueDate,
                command.UserId);

            Result<int> result = await _taskItemService.CreateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<CreateTaskItemResult>(result.Error!);

            return Result.Success(new CreateTaskItemResult(result.Value!));
        }
    }
}
