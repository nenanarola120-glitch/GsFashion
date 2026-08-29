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
    public class MenuRepo : IMenuRepository
    {
        private const string _menuSp = "usp_manage_menus";
        private const string _menusByRoleSp = "usp_get_menus_by_role";
        private readonly IDbConnection _context;

        public MenuRepo(DapperContext context)
        {
            _context = context.CreateConnection();
        }

        #region Get Menus By Role (drives the dynamic sidebar)
        public async Task<IEnumerable<MenuModel>> GetMenusByRoleAsync(int roleId)
        {
            var result = await _context.QueryAsync<MenuModel>(
                _menusByRoleSp,
                new { role_id = roleId },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion

        #region Get All (Manage Menus screen)
        public async Task<IEnumerable<MenuModel>> GetAllAsync()
        {
            var result = await _context.QueryAsync<MenuModel>(
                _menuSp,
                new { Type = SPEnum.GetAll.ToString() },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion

        #region Get All dropdown list
        public async Task<IEnumerable<DropDownResponse>> GetMenuDropDown()
        {
            var result = await _context.QueryAsync<DropDownResponse>(
                _menuSp,
                new { Type = SPEnum.DropDown.ToString() },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion

        #region Get By Id
        public async Task<MenuModel?> GetByIdAsync(int menuId)
        {
            var result = await _context.QueryFirstOrDefaultAsync<MenuModel>(
                _menuSp,
                new { Type = SPEnum.GetById.ToString(), menu_id = menuId },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion

        #region Insert
        public async Task<Response> InsertAsync(MenuModel menu)
        {
            var result = await _context.QueryFirstAsync<Response>(
                _menuSp,
                new
                {
                    Type = SPEnum.Insert.ToString(),
                    menu_name = menu.MenuName,
                    parent_menu_id = menu.ParentMenuId,
                    menu_url = menu.MenuUrl,
                    icon_class = menu.IconClass,
                    display_order = menu.DisplayOrder,
                    is_active = menu.IsActive
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion

        #region Update
        public async Task<Response> UpdateAsync(MenuModel menu)
        {
            var result = await _context.QueryFirstAsync<Response>(
                _menuSp,
                new
                {
                    Type = SPEnum.Update.ToString(),
                    menu_id = menu.MenuId,
                    menu_name = menu.MenuName,
                    parent_menu_id = menu.ParentMenuId,
                    menu_url = menu.MenuUrl,
                    icon_class = menu.IconClass,
                    display_order = menu.DisplayOrder,
                    is_active = menu.IsActive
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion

        #region Delete
        public async Task<Response> DeleteAsync(int menuId)
        {
            var result = await _context.QueryFirstAsync<Response>(
                _menuSp,
                new
                {
                    Type = SPEnum.Delete.ToString(),
                    menu_id = menuId
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion
    }
}
