using GsFashion.Repository.Models;
using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.Menu;

namespace GsFashion.Service.Contracts
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleModel>> GetAllAsync();
        Task<RoleModel?> GetByIdAsync(int roleId);
        Task<IEnumerable<DropDownResponse>> GetRoleDropDown();
        Task<Response> InsertAsync(RoleModel role);
        Task<Response> UpdateAsync(RoleModel role);
        Task<Response> DeleteAsync(int roleId);
    }
}
