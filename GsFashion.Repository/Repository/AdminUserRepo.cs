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
    public class AdminUserRepo : IAdminUserRepository
    {
        private const string _adminUserSp = "usp_manage_admin_users";
        private readonly IDbConnection _context;

        public AdminUserRepo(DapperContext context)
        {
            _context = context.CreateConnection();
        }

        #region Get By Username (login lookup — usp_manage_admin_users has no such branch)
        public async Task<AdminUserModel?> GetByUsernameAsync(string username)
        {
            const string sql = @"
                SELECT u.user_id       AS UserId,
                       u.username      AS Username,
                       u.password_hash AS PasswordHash,
                       u.full_name     AS FullName,
                       u.email         AS Email,
                       u.role_id       AS RoleId,
                       r.role_name     AS RoleName,
                       u.is_active     AS IsActive,
                       u.created_at    AS CreatedAt
                FROM admin_users u
                JOIN roles r ON r.role_id = u.role_id
                WHERE u.username = @Username";

            var result = await _context.QueryFirstOrDefaultAsync<AdminUserModel>(
                sql,
                new { Username = username });

            return result;
        }
        #endregion

        #region Get By Id
        public async Task<AdminUserModel?> GetByIdAsync(int userId)
        {
            var result = await _context.QueryFirstOrDefaultAsync<AdminUserModel>(
                _adminUserSp,
                new { Type = SPEnum.GetById.ToString(), user_id = userId },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion

        #region Get All dropdown list
        public async Task<IEnumerable<DropDownResponse>> GetUserDropDown()
        {
            var result = await _context.QueryAsync<DropDownResponse>(
                _adminUserSp,
                new { Type = SPEnum.DropDown.ToString() },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion

        #region Get All
        public async Task<IEnumerable<AdminUserModel>> GetAllAsync()
        {
            var result = await _context.QueryAsync<AdminUserModel>(
                _adminUserSp,
                new { Type = SPEnum.GetAll.ToString() },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion

        #region Insert
        public async Task<Response> InsertAsync(AdminUserModel user)
        {
            var result = await _context.QueryFirstAsync<Response>(
                _adminUserSp,
                new
                {
                    Type = SPEnum.Insert.ToString(),
                    username = user.Username,
                    password_hash = user.PasswordHash,
                    full_name = user.FullName,
                    email = user.Email,
                    role_id = user.RoleId,
                    is_active = user.IsActive
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion

        #region Update
        public async Task<Response> UpdateAsync(AdminUserModel user)
        {
            var result = await _context.QueryFirstAsync<Response>(
                _adminUserSp,
                new
                {
                    Type = SPEnum.Update.ToString(),
                    user_id = user.UserId,
                    username = user.Username,
                    full_name = user.FullName,
                    email = user.Email,
                    role_id = user.RoleId,
                    is_active = user.IsActive
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion

        #region Delete
        public async Task<Response> DeleteAsync(int userId)
        {
            var result = await _context.QueryFirstAsync<Response>(
                _adminUserSp,
                new
                {
                    Type = SPEnum.Delete.ToString(),
                    user_id = userId
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion
    }
}
