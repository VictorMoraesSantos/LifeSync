using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Users.Application.Interfaces;

namespace Users.Application.Features.Users.Queries.GetUser
{
    public record GetUserQueryHandler : IQueryHandler<GetUserQuery, GetUserQueryResult>
    {
        private readonly IUserService _userService;

        public GetUserQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Result<GetUserQueryResult>> Handle(GetUserQuery query, CancellationToken cancellationToken)
        {
            var result = await _userService.GetUserDetailsAsync(query.userId);
            if (!result.IsSuccess)
                return Result.Failure<GetUserQueryResult>(result.Error!);

            return Result.Success(new GetUserQueryResult(result.Value!));
        }
    }
}
