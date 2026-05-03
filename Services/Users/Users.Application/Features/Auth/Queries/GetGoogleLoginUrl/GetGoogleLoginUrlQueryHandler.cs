using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Users.Application.Contracts;

namespace Users.Application.Features.Auth.Queries.GetGoogleLoginUrl
{
    public class GetGoogleLoginUrlQueryHandler : IQueryHandler<GetGoogleLoginUrlQuery, GetGoogleLoginUrlResult>
    {
        private readonly IGoogleAuthService _googleAuthService;

        public GetGoogleLoginUrlQueryHandler(IGoogleAuthService googleAuthService)
        {
            _googleAuthService = googleAuthService;
        }

        public Task<Result<GetGoogleLoginUrlResult>> Handle(GetGoogleLoginUrlQuery query, CancellationToken cancellationToken)
        {
            var urlResult = _googleAuthService.GetLoginUrl(query.State);

            if (!urlResult.IsSuccess)
                return Task.FromResult(Result<GetGoogleLoginUrlResult>.Failure(urlResult.Error));

            return Task.FromResult(Result<GetGoogleLoginUrlResult>.Success(new GetGoogleLoginUrlResult(urlResult.Value!)));
        }
    }
}
