using FlatFinding.Models;
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

        [HttpGet]      
        public IActionResult UserList()
        {
            return View();
        }

        [HttpGet]
        public IActionResult OwnerList()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AdminSearchFlat()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AdminSearchResult()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Notice()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateNotice(int id = 0)
        {
            if(id == 0)
            {
                return View();
            }
            else
            {
                return View();
            }

        }

        public IActionResult UpdateNotice(Notice model)
        {
            return View("Notice");
        }
    }
}
