using GsFashion.Repository.Contracts;
using GsFashion.Repository.Models;
using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.Menu;
using GsFashion.Service.Contracts;

namespace GsFashion.Service.Implementation
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }
        #region Get All
        public async Task<IEnumerable<MenuModel>> GetAllAsync()
        {
            return await _menuRepository.GetAllAsync();
        }
        #endregion

        #region Get By Id
        public async Task<MenuModel?> GetByIdAsync(int menuId)
        {
            return await _menuRepository.GetByIdAsync(menuId);
        }
        #endregion
        public async Task<IEnumerable<DropDownResponse>> GetMenuDropDown()
        {
            return await _menuRepository.GetMenuDropDown();
        }

        #region Insert
        public async Task<Response> InsertAsync(MenuModel menu)
        {
            var result = await _menuRepository.InsertAsync(menu);

            return result;
        }
        #endregion

        #region Update
        public async Task<Response> UpdateAsync(MenuModel menu)
        {
            var result = await _menuRepository.UpdateAsync(menu);

            return result;
        }
        #endregion

        #region Delete
        public async Task<Response> DeleteAsync(int menuId)
        {
            var result = await _menuRepository.DeleteAsync(menuId);

            return result;
        }
        #endregion
        public async Task<List<MenuTreeModel>> GetMenuTreeForRoleAsync(int roleId)
        {
            var flatMenus = (await _menuRepository.GetMenusByRoleAsync(roleId)).ToList();

            var nodesById = flatMenus.ToDictionary(
                m => m.MenuId,
                m => new MenuTreeModel
                {
                    MenuId = m.MenuId,
                    MenuName = m.MenuName,
                    MenuUrl = m.MenuUrl,
                    IconClass = m.IconClass,
                    DisplayOrder = m.DisplayOrder,
                    CanView = m.CanView,
                    CanAdd = m.CanAdd,
                    CanEdit = m.CanEdit,
                    CanDelete = m.CanDelete
                });

            var roots = new List<MenuTreeModel>();

            foreach (var menu in flatMenus)
            {
                var node = nodesById[menu.MenuId];

                if (menu.ParentMenuId.HasValue && nodesById.TryGetValue(menu.ParentMenuId.Value, out var parentNode))
                {
                    // Parent exists in this role's permitted set -> nest under it
                    parentNode.Children.Add(node);
                }
                else
                {
                    // No parent, or parent isn't visible to this role -> treat as top-level
                    roots.Add(node);
                }
            }

            SortTree(roots);
            return roots;
        }

        private static void SortTree(List<MenuTreeModel> nodes)
        {
            nodes.Sort((a, b) => a.DisplayOrder.CompareTo(b.DisplayOrder));
            foreach (var node in nodes)
                SortTree(node.Children);
        }
    }
}
