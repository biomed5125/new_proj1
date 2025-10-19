using Microsoft.AspNetCore.Mvc;

namespace HMS.Dashboards.Areas.Doctor.Controllers
{
    public class NotesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
