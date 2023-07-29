using Microsoft.AspNetCore.Mvc;

namespace FlatFinding.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
