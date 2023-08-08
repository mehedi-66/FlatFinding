using FlatFinding.Data;
using FlatFinding.Models;
using FlatFinding.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FlatFinding.Controllers
{
    [Authorize(Roles = "User")]
    public class PaymentController : Controller
    {
        private readonly FlatFindingContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentController(UserManager<ApplicationUser> userManager, FlatFindingContext context)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int? id)
        {
            var FlatDetail = await _context.Flats
                .FirstOrDefaultAsync(m => m.FlatId == id);
            if (FlatDetail == null)
                return NotFound();

            ViewBag.Cost = FlatDetail.TotalCost;
            ViewBag.FlatId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PaymentGetWay(PaymentViewModel model)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            // If All okay then booked the flat
            // and make flat avilable NO
            // All detail info Pdf shwo to user for download 
            // if success other wise only confirm Message Tost Notification 

            // comission 1000 + (5)


            var FlatDetail = await _context.Flats
                .FirstOrDefaultAsync(m => m.FlatId == model.FlatId);
            if (FlatDetail == null)
                return NotFound();

            FlatBooked flatBooked = new FlatBooked()
            {
                FlatId = model.FlatId,
                OwnerId = FlatDetail.OwnerId,
                UserId = userId,
                FlatCost = FlatDetail.TotalCost,
                FlatProfit = (FlatDetail.TotalCost / 1000) + 20,
                BookingDate = DateTime.Now,
                PaymentId =  new Random().Next(int.MinValue, int.MaxValue).ToString(),
            };

            // Save Booked 
            _context.Add(flatBooked);
            await _context.SaveChangesAsync();

            // Update Flat Info
            FlatDetail.IsBooking = 1;
            _context.Update(FlatDetail);
            await _context.SaveChangesAsync();
            
            // Redicent to User Profile and Report Generate Pdf 
            return View();
        }
    }
}
