using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlatFinding.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PaymentGetWay()
        {
            return View();
        }
    }
}
