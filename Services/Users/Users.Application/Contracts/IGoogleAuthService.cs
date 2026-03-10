using BuildingBlocks.Results;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Users.Application.Contracts
{
    public interface IGoogleAuthService
    {
        Task<Result<Payload>> ValidateIdTokenAsync(string idToken);
    }
}
