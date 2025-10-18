using BuildingBlocks.CQRS.Queries;
using Users.Application.DTOs.User;

namespace Users.Application.Features.Users.Queries.GetUser
{
    public record GetUserQuery(string userId) : IQuery<GetUserQueryResult>;
    public record GetUserQueryResult(UserDTO User);
}
