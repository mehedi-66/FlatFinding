using FlatFinding.Data;
using FlatFinding.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FlatFinding.Controllers
{
    public class HomeController : Controller
    {
        private readonly FlatFindingContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public HomeController(FlatFindingContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,
                                    SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;

        }

        public async Task<IActionResult> Index()
        {
            var BookingFlats = await _context.FlatBookeds.ToListAsync();

            var userRole = await _roleManager.FindByNameAsync("User");
            var ownerRole = await _roleManager.FindByNameAsync("Owner");
            var AllFlats = await _context.Flats.ToListAsync();
            
            var Flats = AllFlats
                        .Where(f => f.IsBooking == 0 && f.Available == "YES")
                        .OrderByDescending(f => f.Views) 
                        .Take(10)
                        .ToList();
            var AreaWise = AllFlats.Where(f => f.IsBooking == 0 && f.Available == "YES")
                            .GroupBy(flat => flat.AreaName)
                            .Select(group => new
                            {
                                AreaName = group.Key,
                                FlatCount = group.Count()
                            })
                            .ToList();

            if (userRole != null && ownerRole != null)
            {
                var users = await _userManager.GetUsersInRoleAsync(userRole.Name);
                var owner = await _userManager.GetUsersInRoleAsync(ownerRole.Name);
                ViewBag.UserCount = users.Count;
                ViewBag.OwnerCount = owner.Count;
            }

            ViewBag.AllFlats = AllFlats.Count();
            ViewBag.BookedFlats = BookingFlats.Count;
            ViewBag.RecomendedFlats = Flats;
            ViewBag.AreaWise = AreaWise;


            return View();
        }

        public async Task<IActionResult> Notice()
        {
            var Notice = await _context.Notices.ToListAsync();
            ViewBag.Notice = Notice;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> suscriber(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                Suscriber sub = new Suscriber()
                {
                    email = email
                };
                _context.Suscribers.Add(sub);
                await _context.SaveChangesAsync();
            }


            return RedirectToAction("Index");
        }



    }
}