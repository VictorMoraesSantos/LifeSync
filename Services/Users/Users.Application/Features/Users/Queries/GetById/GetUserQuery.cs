using BuildingBlocks.CQRS.Request;
using Users.Application.DTOs.User;

namespace Users.Application.Features.Users.Queries.GetUser
{
    public record GetUserQuery(string userId) : IRequest<GetUserQueryResponse>;
    public record GetUserQueryResponse(UserDTO User);
}
