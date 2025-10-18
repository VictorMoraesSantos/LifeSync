using BuildingBlocks.CQRS.Queries;
using Users.Application.DTOs.User;

namespace Users.Application.Features.Users.Queries.GetAllUsers
{
    public record GetAllUsersQuery() : IQuery<GetAllUsersQueryResult>;
    public record GetAllUsersQueryResult(IEnumerable<UserDTO> Users);
}
