using GsFashion.Repository.Models;
using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.Menu;

namespace GsFashion.Repository.Contracts
{
    public interface IMenuRepository
    {
        // Drives the dynamic sidebar: only menus the given role can view
        Task<IEnumerable<MenuModel>> GetMenusByRoleAsync(int roleId);

        // Full CRUD for the "Manage Menus" admin screen
        Task<IEnumerable<DropDownResponse>> GetMenuDropDown();
        Task<IEnumerable<MenuModel>> GetAllAsync();
        Task<MenuModel?> GetByIdAsync(int menuId);
        Task<Response> InsertAsync(MenuModel menu);
        Task<Response> UpdateAsync(MenuModel menu);
        Task<Response> DeleteAsync(int menuId);
    }
}
