using GsFashion.Repository.Models;
using GsFashion.Repository.Models.Menu;

namespace GsFashion.Service.Contracts
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleModel>> GetAllAsync();
        Task<RoleModel?> GetByIdAsync(int roleId);
        Task<IEnumerable<DropDownResponse>> GetRoleDropDown();
        Task<int> InsertAsync(RoleModel role);
        Task UpdateAsync(RoleModel role);
        Task DeleteAsync(int roleId);
    }
}
