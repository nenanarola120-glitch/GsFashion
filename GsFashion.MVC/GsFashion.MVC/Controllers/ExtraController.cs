using GsFashion.Service.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GsFashion.MVC.Controllers
{
    public class ExtraController : Controller
    {
        private readonly IExtraService _extraService;
        public ExtraController(IExtraService extraService)
        {
            _extraService = extraService;
        }
        public async Task<IActionResult> GetDoctorAppointment()
        {
            var data = await _extraService.GetAppointment();
            return View(data);
        }
    }
}
