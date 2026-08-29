using GsFashion.Repository.Models;
using GsFashion.Service.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GsFashion.MVC.ViewComponents
{
    public class SidebarMenuViewComponent : ViewComponent
    {
        private readonly IMenuService _menuService;

        public SidebarMenuViewComponent(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var roleIdClaim = UserClaims.GetRoleId(HttpContext.User);

            if (roleIdClaim is null)
                return View(new List<MenuTreeModel>());

            var menuTree = await _menuService.GetMenuTreeForRoleAsync(roleIdClaim.Value);
            return View(menuTree);
        }
    }

    // Small claim-reading helper shared by controllers/view components
    public static class UserClaims
    {
        public static int? GetRoleId(System.Security.Claims.ClaimsPrincipal user)
        {
            var value = user.FindFirst("RoleId")?.Value;
            return int.TryParse(value, out var roleId) ? roleId : null;
        }
    }
}
