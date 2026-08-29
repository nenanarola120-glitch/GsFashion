using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GsFashion.MVC.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // The sidebar itself is rendered by the SidebarMenu ViewComponent
            // from _Layout.cshtml, so nothing extra to load here for now.
            return View();
        }
    }
}
