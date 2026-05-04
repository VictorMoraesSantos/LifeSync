namespace LifeSyncApp.Models.Profile
{
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
}
