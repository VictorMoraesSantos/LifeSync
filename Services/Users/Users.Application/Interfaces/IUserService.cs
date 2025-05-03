using System.Security.Claims;
using Users.Application.DTOs.User;

namespace Users.Application.Interfaces
{
    public interface IUserService
    {
        // Operações do usuário autenticado
        Task<UserDTO> GetCurrentUserDetailsAsync(ClaimsPrincipal user);
        Task<bool> UpdateCurrentUserProfileAsync(ClaimsPrincipal user, string firstName, string lastName, string email);
        Task<bool> ChangeCurrentUserPasswordAsync(ClaimsPrincipal user, string currentPassword, string newPassword);
        Task<bool> DeleteCurrentUserAsync(ClaimsPrincipal user);

        // Operações administrativas
        Task<UserDTO> GetUserDetailsAsync(string userId);
        Task<IList<UserDTO>> GetAllUsersAsync();
        Task<IList<UserDTO>> GetAllUsersDetailsAsync();
        Task<bool> IsUserEmailUniqueAsync(string email);
        Task<bool> UpdateUserProfileAsync(string userId, string firstName, string lastName, string email);
        Task<bool> DeleteUserAsync(string userId);

        // Roles
        Task<IList<string>> GetUserRolesAsync(string userId);
        Task<IList<string>> GetCurrentUserRolesAsync(ClaimsPrincipal user);
        Task<bool> AssignUserToRolesAsync(string email, IList<string> roles);
        Task<bool> UpdateUserRolesAsync(string email, IList<string> roles);
    }
}
