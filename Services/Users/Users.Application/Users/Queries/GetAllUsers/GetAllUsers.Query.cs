using MediatR;
using Users.Application.DTOs.User;

namespace Users.Application.Users.Queries.GetUsers
{
    public record GetAllUsersQuery() : IRequest<GetAllUsersQueryResponse>;
    public record GetAllUsersQueryResponse(IList<UserDTO> Users);
}
