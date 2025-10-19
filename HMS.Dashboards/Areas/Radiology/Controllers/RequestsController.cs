using Microsoft.AspNetCore.Mvc;

namespace HMS.Dashboards.Areas.Radiology.Controllers
{
    public class RequestsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
