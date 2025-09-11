using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Commands.CreateTaskItem
{
    public class CreateTaskItemCommandHandler : ICommandHandler<CreateTaskItemCommand, CreateTaskItemResult>
    {
        private readonly ITaskItemService _taskItemService;

        public CreateTaskItemCommandHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<Result<CreateTaskItemResult>> Handle(CreateTaskItemCommand command, CancellationToken cancellationToken)
        {
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
