using FlatFinding.Data;
using System.Linq;
using FlatFinding.Models;
using FlatFinding.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using Org.BouncyCastle.Asn1.Ocsp;
using static Org.BouncyCastle.Math.EC.ECCurve;
using MailKit.Net.Smtp;

namespace FlatFinding.Controllers
{
    public class DashboardController : Controller
    {
        private readonly FlatFindingContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DashboardController(FlatFindingContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,
                                    SignInManager<ApplicationUser> signInManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> UserProfile()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var user =await _userManager.FindByIdAsync(userId);

            List< BookingListViewModel> BookedFlat = (
                            from flatBooked in _context.FlatBookeds
                            join flat in _context.Flats on flatBooked.FlatId equals flat.FlatId
                            where flatBooked.UserId == userId
                            select new BookingListViewModel
                            {
                                Picture = flat.Picture,
                                Address = $"H: {flat.HouseNo} R: {flat.RoadNo} S {flat.sectorNo}, {flat.AreaName}",
                                Cost = flat.TotalCost,
                                Room = int.Parse(flat.RoadNo),
                                Type = flat.Types,
                                Date = flatBooked.BookingDate
                            }
                        ).ToList();

            ViewBag.user = user;
            ViewBag.bookedFlat = BookedFlat; 
            return View();
        } 
        
        public async Task<IActionResult> FlatOwnerProfile()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = await _userManager.FindByIdAsync(userId);
            var flats = _context.Flats.Where(f => f.IsBooking == 0).ToList();

            /* List<BookingListViewModel> BookedFlat = (
                             from flatBooked in _context.FlatBookeds
                             join flat in _context.Flats on flatBooked.FlatId equals flat.FlatId
                             where flatBooked.OwnerId == userId
                             select new BookingListViewModel
                             {
                                 Picture = flat.Picture,
                                 UserName = await _userManager.FindByIdAsync(flatBooked.UserId),
                                 Address = $"H: {flat.HouseNo} R: {flat.RoadNo} S {flat.sectorNo}, {flat.AreaName}",
                                 Cost = flat.TotalCost,
                                 Room = int.Parse(flat.RoadNo),
                                 Type = flat.Types,
                                 Date = flatBooked.BookingDate
                             }
                         ).ToList();*/
            var BookedFlats = (
                 from flatBooked in _context.FlatBookeds
                 join flat in _context.Flats on flatBooked.FlatId equals flat.FlatId
                 where flatBooked.OwnerId == userId
                 select new
                 {
                     Flat = flat,
                     FlatBooked = flatBooked
                 }
             ).ToList();

            List<BookingListViewModel> BookedFlat = new List<BookingListViewModel>();
            foreach (var item in BookedFlats)
            {
                var user12 = await _userManager.FindByIdAsync(item.FlatBooked.UserId);
                BookedFlat.Add( new BookingListViewModel
                {
                    Picture = item.Flat.Picture,
                    UserName = user12.Name,
                    Address = user12.Address,
                    Type = user12.PhoneNumber,
                    Cost = item.Flat.TotalCost,
                    Date = item.FlatBooked.BookingDate
                });
                // Now you can use the bookingViewModel or add it to a list if needed
            }

            ViewBag.Flats = flats;
            ViewBag.Booked = BookedFlat;
            ViewBag.user = user;
            return View();
        } 
        
        public IActionResult AdminDashboard()
        {
            return View();
        }

        [HttpGet]      
        public async Task<IActionResult> UserList()
        {
            var userRole = await _roleManager.FindByNameAsync("User");

            if (userRole != null)
            {
                
                var usersInUserRole = await _userManager.GetUsersInRoleAsync(userRole.Name);
                ViewBag.UserList = usersInUserRole;
                ViewBag.Count = usersInUserRole.Count;
                return View();
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> OwnerList()
        {
            var userRole = await _roleManager.FindByNameAsync("Owner");

            if (userRole != null)
            {

                var usersInUserRole = await _userManager.GetUsersInRoleAsync(userRole.Name);
                ViewBag.UserList = usersInUserRole;
                ViewBag.Count = usersInUserRole.Count;
                return View();
            }
            return View();
        }
        public async Task<IActionResult> DeleteUser(string? id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);

            }

            return RedirectToAction("AdminDashboard");
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
        public async Task<IActionResult> Notice()
        {
            var Notice = await _context.Notices.ToListAsync();
            ViewBag.Notice = Notice;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateNotice(int id = 0)
        {
            if(id == 0)
            {
                return View();
            }
            else
            {
                var notice = await _context.Notices
               .FirstOrDefaultAsync(m => m.NoticeId == id);

                return View(notice);
            }

        }

        public async Task<IActionResult> UpdateNotice(Notice model)
        {
            if (ModelState.IsValid)
            {
                if(model.NoticeId == 0)
                {
                    _context.Add(model);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
               
                return RedirectToAction("Notice");
            }
                return View("CreateNotice", model );
        }

        public async Task<IActionResult> NoticeDelete(int id)
        {

            var model = await _context.Notices.FindAsync(id);
            if (model != null)
            {
                _context.Notices.Remove(model);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Notice");
        }

        public async Task<IActionResult> Subscriber()
        {
          var subscriber = await _context.Suscribers.ToListAsync();
          ViewBag.Subscriber = subscriber;
            return View();
        }

        public IActionResult SendMailToSubscriber(string message, string subject)
        {
            var subscriber =  _context.Suscribers.ToList();

            if(subscriber != null)
            {
                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("mehedihasan.asp.net@gmail.com", "qfelfkvfiobmwdyn");

                foreach (var userEmail in subscriber)
                {
                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse("mehedihasan.asp.net@gmail.com"));
                    email.To.Add(MailboxAddress.Parse(userEmail.email));
                    email.Subject = subject;
                    email.Body = new TextPart(TextFormat.Html) { Text = message };

                   
                    smtp.Send(email);
                  
                }
                smtp.Disconnect(true);

            }
           
            return RedirectToAction("AdminDashboard");
        }
    }
}
