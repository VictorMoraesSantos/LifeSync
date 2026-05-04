namespace LifeSyncApp.DTOs.Profile
{
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
}
