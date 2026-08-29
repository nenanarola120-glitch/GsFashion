using GsFashion.Repository.Models;

namespace GsFashion.Repository.Contracts
{
    public interface IAdminUserRepository
    {
        Task<AdminUserModel?> GetByUsernameAsync(string username);
        Task<AdminUserModel?> GetByIdAsync(int userId);
        Task<IEnumerable<AdminUserModel>> GetAllAsync();
        Task<int> InsertAsync(AdminUserModel user);
        Task UpdateAsync(AdminUserModel user);
        Task DeleteAsync(int userId);
    }
}
