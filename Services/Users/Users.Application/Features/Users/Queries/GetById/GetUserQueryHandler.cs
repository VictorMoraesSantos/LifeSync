using BuildingBlocks.CQRS.Request;
using Users.Application.DTOs.User;
using Users.Application.Interfaces;

namespace Users.Application.Features.Users.Queries.GetUser
{
    public record GetUserQueryHandler : IRequestHandler<GetUserQuery, GetUserQueryResponse>
    {
        private readonly IUserService _userService;

        public GetUserQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<GetUserQueryResponse> Handle(GetUserQuery query, CancellationToken cancellationToken)
        {
            UserDTO result = await _userService.GetUserDetailsAsync(query.userId);
            GetUserQueryResponse response = new(result);
            return response;
        }
    }
}
