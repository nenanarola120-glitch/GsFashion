using GsFashion.Repository.Models;
using GsFashion.Repository.Models.Common;

namespace GsFashion.Repository.Contracts
{
    public interface IRoleMenuPermissionRepository
    {
        // Joined with roles/menus so the Index page can show names, not just ids
        Task<IEnumerable<RoleMenuPermissionModel>> GetAllWithDetailsAsync();

        Task<RoleMenuPermissionModel?> GetByIdAsync(int permissionId);
        Task<Response> InsertAsync(RoleMenuPermissionModel permission);
        Task<Response> UpdateAsync(RoleMenuPermissionModel permission);
        Task<Response> DeleteAsync(int permissionId);
    }
}
