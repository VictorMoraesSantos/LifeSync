namespace Users.Application.DTOs.User
{
    public record UserDTO(
        string Id,
        string FirstName,
        string LastName,
        string FullName,
        string Email,
        DateOnly? BirthDate,
        DateTime CreatedAt,
        DateTime? LastLoginAt,
        bool IsActive,
        IList<string> Roles
    );
}
