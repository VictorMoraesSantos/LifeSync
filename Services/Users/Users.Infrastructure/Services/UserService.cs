﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Users.Application.DTOs.User;
using Users.Application.Interfaces;
using Users.Application.Mapping;
using Users.Domain.Entities;
using Users.Domain.ValueObjects;

namespace Users.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public UserService(
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> ChangeCurrentUserPasswordAsync(ClaimsPrincipal userPrincipal, string currentPassword, string newPassword)
        {
            User user = await _userManager.GetUserAsync(userPrincipal);
            if (user == null) return false;

            IdentityResult result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result.Succeeded;
        }

        public async Task<bool> DeleteCurrentUserAsync(ClaimsPrincipal userPrincipal)
        {
            User user = await _userManager.GetUserAsync(userPrincipal);
            if (user == null) return false;

            IdentityResult result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            IdentityResult result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<IList<UserDTO>> GetAllUsersAsync()
        {
            IList<User> users = await _userManager.Users.AsNoTracking().ToListAsync();
            IList<UserDTO> userDTOs = users.Select(u => UserMapper.ToDto(u)).ToList();
            return userDTOs;
        }

        public async Task<IList<UserDTO>> GetAllUsersDetailsAsync()
        {
            IList<User> users = await _userManager.Users.AsNoTracking().ToListAsync();
            IList<UserDTO> userDTOs = users.Select(u => UserMapper.ToDto(u)).ToList();
            return userDTOs;
        }

        public async Task<UserDTO> GetCurrentUserDetailsAsync(ClaimsPrincipal userPrincipal)
        {
            User user = await _userManager.GetUserAsync(userPrincipal);
            if (user == null) return null;

            IList<string> roles = await _userManager.GetRolesAsync(user);
            UserDTO userDTO = UserMapper.ToDto(user);

            return userDTO;
        }

        public async Task<UserDTO> GetUserDetailsAsync(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            IList<string> roles = await _userManager.GetRolesAsync(user);
            UserDTO userDTO = UserMapper.ToDto(user);
            userDTO = userDTO with { Roles = roles.ToList() };

            return userDTO;
        }


        public async Task<bool> IsUserEmailUniqueAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user == null;
        }

        public async Task<bool> UpdateCurrentUserProfileAsync(ClaimsPrincipal userPrincipal, string firstName, string lastName, string email)
        {
            User user = await _userManager.GetUserAsync(userPrincipal);
            if (user == null) return false;

            Name name = new Name(firstName, lastName);
            Contact contact = new Contact(email);

            user.UpdateProfile(name, contact);

            IdentityResult result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UpdateUserProfileAsync(UpdateUserDTO dto)
        {
            User user = await _userManager.FindByIdAsync(dto.Id);
            if (user == null) return false;

            Name name = new Name(dto.FirstName, dto.LastName);
            Contact contact = new Contact(dto.Email);

            user.UpdateProfile(name, contact);
            IdentityResult result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }


    }
}