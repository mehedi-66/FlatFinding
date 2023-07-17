using Microsoft.AspNetCore.Mvc;

namespace FlatFinding.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult UserProfile()
        {
            return View();
        }
    }
}
