using GsFashion.Repository.Models;

namespace GsFashion.Service.Contracts
{
    public interface IMenuService
    {
        // Flat list from usp_get_menus_by_role, nested into parent/children for the sidebar
        Task<List<MenuTreeModel>> GetMenuTreeForRoleAsync(int roleId);
    }
}
