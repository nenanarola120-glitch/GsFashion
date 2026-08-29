using GsFashion.Repository.Models;
using GsFashion.Repository.Models.Menu;

namespace GsFashion.Repository.Contracts
{
    public interface IRoleRepository
    {
        Task<IEnumerable<RoleModel>> GetAllAsync();
        Task<RoleModel?> GetByIdAsync(int roleId);
        Task<IEnumerable<DropDownResponse>> GetRoleDropDown();
        Task<int> InsertAsync(RoleModel role);
        Task UpdateAsync(RoleModel role);
        Task DeleteAsync(int roleId);
    }
}
