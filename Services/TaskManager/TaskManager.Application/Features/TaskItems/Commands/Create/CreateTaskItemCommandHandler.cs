using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Commands.Create
{
    public class CreateTaskItemCommandHandler :
        SecureCommandHandlerBase,
        ICommandHandler<CreateTaskItemCommand, CreateTaskItemResult>
    {
        private readonly ITaskItemService _taskItemService;
        private readonly IValidator<CreateTaskItemCommand> _validator;

        public CreateTaskItemCommandHandler(
            ITaskItemService taskItemService,
            IValidator<CreateTaskItemCommand> validator,
            IHttpContextAccessor httpContext) : base(httpContext)
        {
            _taskItemService = taskItemService;
            _validator = validator;
        }

        public async Task<Result<CreateTaskItemResult>> Handle(CreateTaskItemCommand command, CancellationToken cancellationToken)
        {
            Result? ownershipValidation = ValidateOwnership(command.UserId);
            if (!ownershipValidation.IsSuccess)
                return Result.Failure<CreateTaskItemResult>(ownershipValidation.Error!);

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
                command.UserId,
                command.TaskLabelsId);

            Result<int> result = await _taskItemService.CreateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<CreateTaskItemResult>(result.Error!);

            return Result.Success(new CreateTaskItemResult(result.Value!));
        }
    }
}
