using GsFashion.Service.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GsFashion.MVC.Controllers
{
    public class ExtraController : Controller
    {
        private readonly IExtraService extraService;
        public ExtraController(IExtraService extraService)
        {
            extraService = extraService;
        }
        public async Task<IActionResult> GetDoctorAppointment()
        {
            return View(await extraService.GetAppointment());
        }
    }
}
