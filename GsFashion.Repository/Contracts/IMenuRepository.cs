using GsFashion.Repository.Models;

namespace GsFashion.Repository.Contracts
{
    public interface IMenuRepository
    {
        // Drives the dynamic sidebar: only menus the given role can view
        Task<IEnumerable<MenuModel>> GetMenusByRoleAsync(int roleId);

        // Full CRUD for the "Manage Menus" admin screen
        Task<IEnumerable<MenuModel>> GetAllAsync();
        Task<MenuModel?> GetByIdAsync(int menuId);
        Task<int> InsertAsync(MenuModel menu);
        Task UpdateAsync(MenuModel menu);
        Task DeleteAsync(int menuId);
    }
}
