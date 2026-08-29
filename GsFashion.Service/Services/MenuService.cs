using GsFashion.Repository.Contracts;
using GsFashion.Repository.Models;
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
