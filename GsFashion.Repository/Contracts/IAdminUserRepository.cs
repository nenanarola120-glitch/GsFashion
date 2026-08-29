using GsFashion.Repository.Models;
using GsFashion.Repository.Models.Common;

namespace GsFashion.Repository.Contracts
{
    public interface IAdminUserRepository
    {
        Task<AdminUserModel?> GetByUsernameAsync(string username);
        Task<AdminUserModel?> GetByIdAsync(int userId);
        Task<IEnumerable<AdminUserModel>> GetAllAsync();
        Task<Response> InsertAsync(AdminUserModel user);
        Task<Response> UpdateAsync(AdminUserModel user);
        Task<Response> DeleteAsync(int userId);
    }
}
