using GsFashion.Repository.Models;
using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.Menu;

namespace GsFashion.Service.Contracts
{
    public interface IMenuService
    {
        // Get all menus
        Task<IEnumerable<MenuModel>> GetAllAsync();

        // Get menu by ID
        Task<MenuModel?> GetByIdAsync(int menuId);

        Task<IEnumerable<DropDownResponse>> GetMenuDropDown();
        // Insert menu
        Task<Response> InsertAsync(MenuModel menu);

        // Update menu
        Task<Response> UpdateAsync(MenuModel menu);

        // Delete menu
        Task<Response> DeleteAsync(int menuId);
        // Flat list from usp_get_menus_by_role, nested into parent/children for the sidebar
        Task<List<MenuTreeModel>> GetMenuTreeForRoleAsync(int roleId);

  
    }
}
