using GsFashion.Repository.Contracts;
using GsFashion.Repository.Extension;
using GsFashion.Repository.Models;
using GsFashion.Repository.Models.Common;
using GsFashion.Service.Contracts;

namespace GsFashion.Service.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IAdminUserRepository _adminUserRepository;

        public AuthService(IAdminUserRepository adminUserRepository)
        {
            _adminUserRepository = adminUserRepository;
        }

        public async Task<AdminUserModel?> ValidateLoginAsync(string username, string password)
        {
            var user = await _adminUserRepository.GetByUsernameAsync(username);

            if (user is null || !user.IsActive)
                return null;

            if (!PasswordHasher.Verify(password, user.PasswordHash))
                return null;

            return user;
        }

        public async Task<Response> RegisterAsync(string username,string password,string? fullName,string? email,int roleId)
        {
            var existing = await _adminUserRepository.GetByUsernameAsync(username);

            if (existing is not null)
            {
                return new Response
                {
                    Status = 0,
                    Message = "That username is already taken."
                };
            }

            var newUser = new AdminUserModel
            {
                Username = username,
                PasswordHash = PasswordHasher.Hash(password),
                FullName = fullName,
                Email = email,
                RoleId = roleId,
                IsActive = true
            };

            var result = await _adminUserRepository.InsertAsync(newUser);

            return result;
        }
    }
}
