using GsFashion.Repository.Models;
using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.Menu;

namespace GsFashion.Service.Contracts
{
    public interface IAdminUserService
    {
        Task<AdminUserModel?> GetByUsernameAsync(string username);
        Task<AdminUserModel?> GetByIdAsync(int userId);
        Task<IEnumerable<DropDownResponse>> GetUserDropDown();
        Task<IEnumerable<AdminUserModel>> GetAllAsync();
        Task<Response> InsertAsync(AdminUserModel user);
        Task<Response> UpdateAsync(AdminUserModel user);
        Task<Response> DeleteAsync(int userId);
    }

}
