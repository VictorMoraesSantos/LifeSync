using BuildingBlocks.Results;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Users.Application.Contracts
{
    public interface IGoogleAuthService
    {
        Result<string> GetLoginUrl(string? state);
        Task<Result<string>> ExchangeCodeForIdTokenAsync(string code, CancellationToken cancellationToken = default);
        Task<Result<Payload>> ValidateIdTokenAsync(string idToken);
    }
}
