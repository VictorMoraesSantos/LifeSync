using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Users.Application.Interfaces;

namespace Users.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, GetAllUsersQueryResult>
    {
        private readonly IUserService _userService;

        public GetAllUsersQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Result<GetAllUsersQueryResult>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            var result = await _userService.GetAllUsersDetailsAsync();
            if (!result.IsSuccess)
                return Result.Failure<GetAllUsersQueryResult>(result.Error!);

            return Result.Success(new GetAllUsersQueryResult(result.Value!));
        }
    }
}
