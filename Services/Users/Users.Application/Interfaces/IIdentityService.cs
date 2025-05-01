using Users.Application.DTOs.Role;
using Users.Application.DTOs.User;

namespace Users.Application.Interfaces
{
    public interface IIdentityService
    {
        // User section
        Task<string> CreateUserAsync(string userName, string password, string email, string firstName, string lastName, IList<string> roles);
        Task<bool> SignInUserAsync(string email, string password);
        Task<string> GetUserIdAsync(string email);
        Task<UserDetailsDTO> GetUserDetailsAsync(string userId);
        Task<UserDetailsDTO> GetUserDetailsByUserNameAsync(string userName);
        Task<string> GetUserNameAsync(string userId);
        Task<bool> DeleteUserAsync(string userId);
        Task<bool> IsUserNameUniqueAsync(string userName);
        Task<IList<UserSummaryDTO>> GetAllUsersAsync();
        Task<IList<UserDetailsDTO>> GetAllUsersDetailsAsync();
        Task<bool> UpdateUserProfileAsync(string userId, string firstName, string lastName, string email, IList<string> roles);

        // Role section
        Task<bool> CreateRoleAsync(string roleName);
        Task<bool> DeleteRoleAsync(string roleId);
        Task<IList<RoleDTO>> GetRolesAsync();
        Task<RoleDTO> GetRoleByIdAsync(string roleId);
        Task<bool> UpdateRoleAsync(string roleId, string roleName);

        // User's Role section
        Task<bool> IsUserInRoleAsync(string userId, string role);
        Task<IList<string>> GetUserRolesAsync(string userId);
        Task<bool> AssignUserToRolesAsync(string userName, IList<string> roles);
        Task<bool> UpdateUserRolesAsync(string userName, IList<string> roles);

        // Refresh Token section
        Task<bool> UpdateUserRefreshTokenAsync(string userId, string refreshToken, DateTime expiryTime);
        Task<bool> RevokeUserRefreshTokenAsync(string userId);
        Task<string> GetUserRefreshTokenAsync(string userId);
    }
}

