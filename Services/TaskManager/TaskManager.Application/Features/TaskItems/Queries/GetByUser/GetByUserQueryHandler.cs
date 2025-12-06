using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Http;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Queries.GetByUser
{
    public class GetByUserQueryHandler :
        SecureQueryHandlerBase,
        IQueryHandler<GetByUserQuery, GetByUserResponse>
    {
        private readonly ITaskItemService _taskItemService;

        public GetByUserQueryHandler(
            ITaskItemService taskItemService,
            IHttpContextAccessor httpContext) : base(httpContext)
        {
            _taskItemService = taskItemService;
        }

        public async Task<Result<GetByUserResponse>> Handle(GetByUserQuery query, CancellationToken cancellationToken)
        {
            var validation = ValidateAccess<GetByUserResponse>(query.UserId);
            if (!validation.IsSuccess)
                return validation;

            var result = await _taskItemService.FindAsync(t => t.UserId == query.UserId, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetByUserResponse>(result.Error!);

            return Result.Success(new GetByUserResponse(result.Value!));
        }
    }
}
