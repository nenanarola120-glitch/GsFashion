using GsFashion.Repository.Models;
using GsFashion.Repository.Models.Common;

namespace GsFashion.Service.Contracts
{
   public interface IAuthService
    {
        // Returns the user when username/password are valid and the account is active,
        // otherwise null. Controller is responsible for signing the cookie in.
        Task<AdminUserModel?> ValidateLoginAsync(string username, string password);

        // Creates a new admin_users row. Hashes the password and rejects duplicate usernames.
        Task<Response> RegisterAsync(string username, string password, string? fullName, string? email, int roleId);
    }
}
