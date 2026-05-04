namespace LifeSyncApp.Models.Auth
{
    public record UserDTO(
        string Id,
        string FirstName,
        string LastName,
        string FullName,
        string Email,
        bool IsActive,
        IList<string> Roles);
}
