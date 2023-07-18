using Microsoft.AspNetCore.Mvc;

namespace FlatFinding.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult UserProfile()
        {
            return View();
        } 
        
        public IActionResult FlatOwnerProfile()
        {
            return View();
        } 
        
        public IActionResult AdminDashboard()
        {
            return View();
        }
    }
}
