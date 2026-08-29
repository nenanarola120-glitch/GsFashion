using GsFashion.Repository.Models;

namespace GsFashion.Service.Contracts
{
    public class RegisterResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public static RegisterResult Ok() => new() { Success = true };
        public static RegisterResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    }

    public interface IAuthService
    {
        // Returns the user when username/password are valid and the account is active,
        // otherwise null. Controller is responsible for signing the cookie in.
        Task<AdminUserModel?> ValidateLoginAsync(string username, string password);

        // Creates a new admin_users row. Hashes the password and rejects duplicate usernames.
        Task<RegisterResult> RegisterAsync(string username, string password, string? fullName, string? email, int roleId);
    }
}
