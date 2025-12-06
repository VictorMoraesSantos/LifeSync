using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Http;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Queries.GetById
{
    public class GetTaskItemByIdQueryHandler :
        SecureQueryHandlerBase,
        IQueryHandler<GetTaskItemByIdQuery, GetTaskItemByIdResult>
    {
        private readonly ITaskItemService _taskItemService;

        public GetTaskItemByIdQueryHandler(
            ITaskItemService taskItemService,
            IHttpContextAccessor httpContext) : base(httpContext)
        {
            _taskItemService = taskItemService;
        }

        public async Task<Result<GetTaskItemByIdResult>> Handle(GetTaskItemByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _taskItemService.GetByIdAsync(query.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetTaskItemByIdResult>(result.Error!);

            var validation = ValidateAccess<GetTaskItemByIdResult>(result.Value!.UserId);
            if (!validation.IsSuccess)
                return validation;

            return Result.Success(new GetTaskItemByIdResult(result.Value!));
        }
    }
}
