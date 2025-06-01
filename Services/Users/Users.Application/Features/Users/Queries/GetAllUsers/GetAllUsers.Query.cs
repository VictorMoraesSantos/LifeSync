using BuildingBlocks.CQRS.Request;
using Users.Application.DTOs.User;

namespace Users.Application.Features.Users.Queries.GetAllUsers
{
    public record GetAllUsersQuery() : IRequest<GetAllUsersQueryResponse>;
    public record GetAllUsersQueryResponse(IList<UserDTO> Users);
}
