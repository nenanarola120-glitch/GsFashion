using Dapper;
using GsFashion.Repository.Contracts;
using GsFashion.Repository.Dapper;
using GsFashion.Repository.Enums;
using GsFashion.Repository.Models;
using GsFashion.Repository.Models.Common;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace GsFashion.Repository.Repository
{
    public class RoleMenuPermissionRepo : IRoleMenuPermissionRepository
    {
        private const string _rmpSp = "usp_manage_role_menu_permissions";
        private readonly IDbConnection _context;

        public RoleMenuPermissionRepo(DapperContext context)
        {
            _context = context.CreateConnection();
        }

        #region Get All With Details (joined for the Index page)
        public async Task<IEnumerable<RoleMenuPermissionModel>> GetAllWithDetailsAsync()
        {
            const string sql = @"
                SELECT rmp.permission_id AS PermissionId,
                       rmp.role_id       AS RoleId,
                       r.role_name       AS RoleName,
                       rmp.menu_id       AS MenuId,
                       m.menu_name       AS MenuName,
                       rmp.can_view      AS CanView,
                       rmp.can_add       AS CanAdd,
                       rmp.can_edit      AS CanEdit,
                       rmp.can_delete    AS CanDelete,
                       rmp.created_at    AS CreatedAt
                FROM role_menu_permissions rmp
                JOIN roles r ON r.role_id = rmp.role_id
                JOIN menus m ON m.menu_id = rmp.menu_id
                ORDER BY r.role_name, m.display_order, m.menu_id";

            var result = await _context.QueryAsync<RoleMenuPermissionModel>(sql);
            return result;
        }
        #endregion

        #region Get By Id
        public async Task<RoleMenuPermissionModel?> GetByIdAsync(int permissionId)
        {
            var result = await _context.QueryFirstOrDefaultAsync<RoleMenuPermissionModel>(
                _rmpSp,
                new { Type = SPEnum.GetById.ToString(), permission_id = permissionId },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion

        #region Insert
        public async Task<Response> InsertAsync(RoleMenuPermissionModel permission)
        {
            var result = await _context.QueryFirstAsync<Response>(
                _rmpSp,
                new
                {
                    Type = SPEnum.Insert.ToString(),
                    role_id = permission.RoleId,
                    menu_id = permission.MenuId,
                    can_view = permission.CanView,
                    can_add = permission.CanAdd,
                    can_edit = permission.CanEdit,
                    can_delete = permission.CanDelete
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion

        #region Update
        public async Task<Response> UpdateAsync(RoleMenuPermissionModel permission)
        {
            var result = await _context.QueryFirstAsync<Response>(
                _rmpSp,
                new
                {
                    Type = SPEnum.Update.ToString(),
                    permission_id = permission.PermissionId,
                    role_id = permission.RoleId,
                    menu_id = permission.MenuId,
                    can_view = permission.CanView,
                    can_add = permission.CanAdd,
                    can_edit = permission.CanEdit,
                    can_delete = permission.CanDelete
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion

        #region Delete
        public async Task<Response> DeleteAsync(int permissionId)
        {
            var result = await _context.QueryFirstAsync<Response>(
                _rmpSp,
                new
                {
                    Type = SPEnum.Delete.ToString(),
                    permission_id = permissionId
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion
    }
}
