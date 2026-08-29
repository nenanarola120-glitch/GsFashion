using Dapper;
using GsFashion.Repository.Contracts;
using GsFashion.Repository.Dapper;
using GsFashion.Repository.Enums;
using GsFashion.Repository.Models;
using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.Menu;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace GsFashion.Repository.Repository
{
    public class RoleRepo : IRoleRepository
    {
        private const string _roleSp = "usp_manage_roles";
        private readonly IDbConnection _context;

        public RoleRepo(DapperContext context)
        {
            _context = context.CreateConnection();
        }

        #region Get All
        public async Task<IEnumerable<RoleModel>> GetAllAsync()
        {
            var result = await _context.QueryAsync<RoleModel>(
                _roleSp,
                new { Type = SPEnum.GetAll.ToString() },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion

        #region Get All dropdown list
        public async Task<IEnumerable<DropDownResponse>> GetRoleDropDown()
        {
            var result = await _context.QueryAsync<DropDownResponse>(
                _roleSp,
                new { Type = SPEnum.DropDown.ToString() },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion

        #region Get By Id
        public async Task<RoleModel?> GetByIdAsync(int roleId)
        {
            var result = await _context.QueryFirstOrDefaultAsync<RoleModel>(
                _roleSp,
                new { Type = SPEnum.GetById.ToString(), role_id = roleId },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion

        #region Insert
        public async Task<Response> InsertAsync(RoleModel role)
        {
            var result = await _context.QueryFirstAsync<Response>(
                _roleSp,
                new
                {
                    Type = SPEnum.Insert.ToString(),
                    role_name = role.RoleName,
                    description = role.Description,
                    is_active = role.IsActive
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion

        #region Update
        public async Task<Response> UpdateAsync(RoleModel role)
        {
            var result = await _context.QueryFirstAsync<Response>(
                _roleSp,
                new
                {
                    Type = SPEnum.Update.ToString(),
                    role_id = role.RoleId,
                    role_name = role.RoleName,
                    description = role.Description,
                    is_active = role.IsActive
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion

        #region Delete
        public async Task<Response> DeleteAsync(int roleId)
        {
            var result = await _context.QueryFirstAsync<Response>(
                _roleSp,
                new
                {
                    Type = SPEnum.Delete.ToString(),
                    role_id = roleId
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion
    }
}
