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

        private readonly IRoleMenuPermissionRepository _repository;

        public RoleMenuPermissionService(IRoleMenuPermissionRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<RoleMenuPermissionModel>> GetAllAsync()
            => _repository.GetAllWithDetailsAsync();

        public Task<RoleMenuPermissionModel?> GetByIdAsync(int permissionId)
            => _repository.GetByIdAsync(permissionId);

        #region Create
        public async Task<Response> CreateAsync(RoleMenuPermissionModel permission)
        {
            try
            {
                var result = await _repository.InsertAsync(permission);

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
                var result = await _repository.UpdateAsync(permission);

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
                var result = await _repository.DeleteAsync(permissionId);

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
    }
}
