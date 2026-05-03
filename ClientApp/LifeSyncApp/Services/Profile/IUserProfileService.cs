using LifeSyncApp.DTOs.Auth;
using LifeSyncApp.DTOs.Profile;

namespace LifeSyncApp.Services.Profile
{
    public interface IUserProfileService
    {
        Task<UserDTO?> GetUserAsync(int userId, CancellationToken cancellationToken = default);
        Task<(bool Success, string? Error)> UpdateUserAsync(int userId, UpdateUserRequest dto, CancellationToken cancellationToken = default);
        Task<(bool Success, string? Error)> ChangePasswordAsync(ChangePasswordRequest dto, CancellationToken cancellationToken = default);
    }
}
