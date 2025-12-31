using Microsoft.AspNetCore.Mvc;

namespace MOM.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
