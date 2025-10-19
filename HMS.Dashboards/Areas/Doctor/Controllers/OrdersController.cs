using Microsoft.AspNetCore.Mvc;

namespace HMS.Dashboards.Areas.Doctor.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
