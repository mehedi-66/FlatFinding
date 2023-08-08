using FlatFinding.Data;
using FlatFinding.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FlatFinding.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FlatFindingContext _context;
        public HomeController(ILogger<HomeController> logger, FlatFindingContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var RecomendedFlats = await _context.Flats.ToListAsync();
            ViewBag.RecomendedFlats = RecomendedFlats;

            return View();
        }


     
    }
}