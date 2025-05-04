using MediatR;
using Users.Application.DTOs.User;
using Users.Application.Interfaces;

namespace Users.Application.Users.Queries.GetUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, GetAllUsersQueryResponse>
    {
        private readonly IUserService _userService;

        public GetAllUsersQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<GetAllUsersQueryResponse> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            IList<UserDTO> result = await _userService.GetAllUsersDetailsAsync();
            GetAllUsersQueryResponse response = new(result);
            return response;
        }
    }
}
