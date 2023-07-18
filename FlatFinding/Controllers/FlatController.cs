using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlatFinding.Controllers
{
    [Authorize]
    public class FlatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FlatDetails()
        {
            return View();
        }  
        public IActionResult Create()
        {
            return View();
        }
        
        public IActionResult Update()
        {
            return View();
        }
    }
}
