using Microsoft.AspNetCore.Mvc;

namespace HMS.Dashboards.Controllers
{
    // HMS.Dashboards/Controllers/DoctorController.cs
    public class DoctorController : Controller
    {
        private readonly IConfiguration _cfg;
        public DoctorController(IConfiguration cfg) => _cfg = cfg;

        public IActionResult EyeBird()
        {
            // fallback to same-origin if you prefer
            ViewData["ApiBase"] = _cfg["Api:BaseUrl"]
                                  ?? $"{Request.Scheme}://{Request.Host}";
            return View();
        }
    }

}
