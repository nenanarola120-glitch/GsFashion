using GsFashion.Repository.Models;
using GsFashion.Repository.Models.Common;

namespace GsFashion.Service.Contracts
{
    public interface IRoleMenuPermissionService
    {
        Task<IEnumerable<RoleMenuPermissionModel>> GetAllAsync();
        Task<RoleMenuPermissionModel?> GetByIdAsync(int permissionId);
        Task<Response> CreateAsync(RoleMenuPermissionModel permission);
        Task<Response> UpdateAsync(RoleMenuPermissionModel permission);
        Task<Response> DeleteAsync(int permissionId);
        Task<List<PermissionMatrixRow>> GetPermissionMatrixAsync(int roleId);
        Task<Response> ApplyPermissionMatrixAsync(int roleId, List<PermissionMatrixRow> rows);
    }
}
