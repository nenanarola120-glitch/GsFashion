using GsFashion.Repository.Contracts;
using GsFashion.Repository.Models;
using GsFashion.Repository.Models.Common;
using GsFashion.Service.Contracts;
using Microsoft.Data.SqlClient;

namespace GsFashion.Service.Implementation
{
    public class RoleMenuPermissionService : IRoleMenuPermissionService
    {
        // SQL Server error numbers for unique-constraint / unique-index violations
        private const int UniqueConstraintViolation = 2627;
        private const int UniqueIndexViolation = 2601;

        private readonly IRoleMenuPermissionRepository _roleMenuPermissionRepository;
        private readonly IMenuRepository _menuRepository;

        public RoleMenuPermissionService(IRoleMenuPermissionRepository repository, IMenuRepository menuRepository)
        {
            _roleMenuPermissionRepository = repository;
            _menuRepository = menuRepository;
        }

        public Task<IEnumerable<RoleMenuPermissionModel>> GetAllAsync()
            => _roleMenuPermissionRepository.GetAllWithDetailsAsync();

        public Task<RoleMenuPermissionModel?> GetByIdAsync(int permissionId)
            => _roleMenuPermissionRepository.GetByIdAsync(permissionId);

        #region Create
        public async Task<Response> CreateAsync(RoleMenuPermissionModel permission)
        {
            try
            {
                var result = await _roleMenuPermissionRepository.InsertAsync(permission);

                return result;
            }
            catch (SqlException ex) when (
                ex.Number is UniqueConstraintViolation or UniqueIndexViolation)
            {
                return new Response
                {
                    Status = 0,
                    Message = "A permission entry for this role and menu already exists. Edit that entry instead."
                };
            }
        }
        #endregion

        #region Update
        public async Task<Response> UpdateAsync(RoleMenuPermissionModel permission)
        {
            try
            {
                var result = await _roleMenuPermissionRepository.UpdateAsync(permission);

                return result;
            }
            catch (SqlException ex) when (
                ex.Number is UniqueConstraintViolation or UniqueIndexViolation)
            {
                return new Response
                {
                    Status = 0,
                    Message = "Another permission entry already exists for this role and menu."
                };
            }
        }
        #endregion

        #region Delete
        public async Task<Response> DeleteAsync(int permissionId)
        {
            try
            {
                var result = await _roleMenuPermissionRepository.DeleteAsync(permissionId);

                return result;
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Status = 0,
                    Message = ex.Message
                };
            }
        }
        #endregion

        public async Task<List<PermissionMatrixRow>> GetPermissionMatrixAsync(int roleId)
        {
            // must be the FULL menu list (with real MenuId), not GetMenuDropDown() reused for something else
            var allMenus = (await _menuRepository.GetAllAsync())
                .Where(m => m.IsActive)
                .OrderBy(m => m.DisplayOrder)
                .ToList();

            // must be the JOINED table data (has CanView/CanAdd/CanEdit/CanDelete), not a dropdown call
            var existingForRole = (await _roleMenuPermissionRepository.GetAllWithDetailsAsync())
                .Where(p => p.RoleId == roleId)
                .ToList();

            // TEMP DEBUG — drop a breakpoint or Console.WriteLine here to confirm existingForRole.Count > 0
            // and that at least one row has CanView == true for this roleId.

            return allMenus.Select(menu =>
            {
                var existing = existingForRole.FirstOrDefault(p => p.MenuId == menu.MenuId);
                return new PermissionMatrixRow
                {
                    MenuId = menu.MenuId,
                    MenuName = menu.MenuName,
                    CanView = existing?.CanView ?? false,
                    CanAdd = existing?.CanAdd ?? false,
                    CanEdit = existing?.CanEdit ?? false,
                    CanDelete = existing?.CanDelete ?? false
                };
            }).ToList();
        }

        public async Task<Response> ApplyPermissionMatrixAsync(int roleId, List<PermissionMatrixRow> rows)
        {
            try
            {
                var existingForRole = (await _roleMenuPermissionRepository.GetAllWithDetailsAsync())
                    .Where(p => p.RoleId == roleId)
                    .ToList();

                foreach (var row in rows)
                {
                    var existing = existingForRole.FirstOrDefault(p => p.MenuId == row.MenuId);
                    bool anyChecked = row.CanView || row.CanAdd || row.CanEdit || row.CanDelete;

                    if (anyChecked)
                    {
                        if (existing is not null)
                        {
                            existing.CanView = row.CanView;
                            existing.CanAdd = row.CanAdd;
                            existing.CanEdit = row.CanEdit;
                            existing.CanDelete = row.CanDelete;
                            await _roleMenuPermissionRepository.UpdateAsync(existing);
                        }
                        else
                        {
                            await _roleMenuPermissionRepository.InsertAsync(new RoleMenuPermissionModel
                            {
                                RoleId = roleId,
                                MenuId = row.MenuId,
                                CanView = row.CanView,
                                CanAdd = row.CanAdd,
                                CanEdit = row.CanEdit,
                                CanDelete = row.CanDelete
                            });
                        }
                    }
                    else if (existing is not null)
                    {
                        await _roleMenuPermissionRepository.DeleteAsync(existing.PermissionId);
                    }
                }

                return new Response { Message = "Permissions saved successfully", Status = 1 };
            }
            catch (SqlException ex) when (ex.Number is 2627 or 2601)
            {
                return new Response { Message = "A permission conflict occurred while saving. Please retry.", Status = 0 };
            }
        }
    }
}
