using MediatR;
using Users.Application.DTOs.User;

namespace Users.Application.Users.Queries.GetUser
{
    public record GetUserQuery(string userId) : IRequest<GetUserQueryResponse>;
    public record GetUserQueryResponse(UserDTO User);
}
