using BuildingBlocks.CQRS.Requests.Queries;

namespace Users.Application.Features.Auth.Queries.GetGoogleLoginUrl
{
    public record GetGoogleLoginUrlQuery(string State) : IQuery<GetGoogleLoginUrlResult>;
    public record GetGoogleLoginUrlResult(string Url);
}
