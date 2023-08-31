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
using DinkToPdf;
using FlatFinding.ReportTemplate;
using DinkToPdf.Contracts;

namespace FlatFinding.Controllers
{
    public class DashboardController : Controller
    {
        private IConverter _converter;
        private readonly FlatFindingContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DashboardController(IConverter converter, FlatFindingContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,
                                    SignInManager<ApplicationUser> signInManager, IWebHostEnvironment webHostEnvironment)
        {
            _converter = converter;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        public IActionResult FlatBookingCancel(int id)
        {
            try
            {
                if (id != 0)
                {
                    var flatBooked = _context.FlatBookeds.FirstOrDefault(f => f.FlatBookedId == id);
                    if (flatBooked != null)
                    {
                        var flat = _context.Flats.AsNoTracking().FirstOrDefault(f => f.FlatId == flatBooked.FlatId);
                        if (flat != null)
                        {
                            flat.FlatId = 0;
                            flat.IsBooking = 0;
                            _context.Flats.Add(flat);
                            _context.SaveChanges();

                            flatBooked.IsDelete = 1;
                            flatBooked.BookingCancel = DateTime.Now;
                            _context.FlatBookeds.Update(flatBooked);
                            _context.SaveChanges();

                           
                        }
                    }
                   
                }
                return RedirectToAction("Index", "Home");
            }
            catch {
                return View("Error");
            }

          
        }

        public async Task<IActionResult> FlatSearchForAdmin(string Area = "", string Type = "", string FlatStatus = "")
        {
            // Admin Search page open 
            try
            {
                List<Flat> flats = new List<Flat>();

               
               // All   All   All
               // Area Fix start 
               if(Area == "All" && Type ==  "All" && FlatStatus != "All")
               {
                    flats = await _context.Flats.Where(f => f.IsBooking == 0 && f.FlatStatus == FlatStatus).ToListAsync();

                }
               else if(Area == "All" && Type != "All" && FlatStatus == "All")
               {
                    flats = await _context.Flats.Where(f => f.IsBooking == 0 && f.Types == Type).ToListAsync();
                }
               else if(Area == "All" && Type != "All" && FlatStatus != "All")
               {
                    flats = await _context.Flats.Where(f => f.IsBooking == 0 && f.Types == Type && f.FlatStatus == FlatStatus).ToListAsync();
               }
                // Area fix end
                // Type Fix start 
                if (Area != "All" && Type == "All" && FlatStatus == "All")
                {
                    flats = await _context.Flats.Where(f => f.IsBooking == 0 && f.AreaName == Area).ToListAsync();

                }
                else if (Area == "All" && Type == "All" && FlatStatus != "All")
                {
                    flats = await _context.Flats.Where(f => f.IsBooking == 0 && f.FlatStatus == FlatStatus).ToListAsync();
                }
                else if (Area != "All" && Type == "All" && FlatStatus != "All")
                {
                    flats = await _context.Flats.Where(f => f.IsBooking == 0 && f.AreaName == Area && f.FlatStatus == FlatStatus).ToListAsync();
                }
                // Type fix end
                // Status Fix start 
                if (Area != "All" && Type == "All" && FlatStatus == "All")
                {
                    flats = await _context.Flats.Where(f => f.IsBooking == 0 && f.AreaName == Area).ToListAsync();

                }
                else if (Area == "All" && Type != "All" && FlatStatus == "All")
                {
                    flats = await _context.Flats.Where(f => f.IsBooking == 0 && f.Types == Type).ToListAsync();
                }
                else if (Area != "All" && Type != "All" && FlatStatus == "All")
                {
                    flats = await _context.Flats.Where(f => f.IsBooking == 0 && f.AreaName == Area && f.Types == Type).ToListAsync();
                }
                // Status fix end



                if ((Area == "" || Area == "All") && (Type == "" || Type == "All") && (FlatStatus == "" || FlatStatus == "All"))
                {
                    flats = await _context.Flats.Where(f => f.IsBooking == 0).ToListAsync();
                }
               
                
                ViewBag.Flats = flats;
                return View();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private int UpdateStatusofFlat(string status, int id)
        {
            var flat = _context.Flats.FirstOrDefault(f => f.FlatId == id);
            if (flat != null)
            {   
                flat.FlatStatus = status;
                _context.Flats.Update(flat);
                _context.SaveChanges();
                return 1;
            }
            else
            {
                return 0;
            }
        }
        [HttpGet]
        public IActionResult AdminUpdateFlatStatusAccepted(int id)
        {
            // Accepted 
            UpdateStatusofFlat("Accepted", id);
            // success 1 and fail 0
            return RedirectToAction("FlatSearchForAdmin");
        }  
        public IActionResult AdminUpdateFlatStatusPending(int id)
        {
            // Pending
            UpdateStatusofFlat("Pending", id);
            return RedirectToAction("FlatSearchForAdmin");
        } 
        public IActionResult AdminUpdateFlatStatusRejected(int id)
        {
            // Rejected 
            UpdateStatusofFlat("Rejected", id);
            return RedirectToAction("FlatSearchForAdmin");
        }
        public IActionResult AdminUpdateFlatStatusDelete(int id)
        {
            // Delete 
            var flat = (_context.Flats.FirstOrDefault(f=>f.FlatId == id));
            if(flat != null)
            {
                _context.Flats.Remove(flat);
                _context.SaveChanges();
            }
            return RedirectToAction("FlatSearchForAdmin");
        }
        public async Task<IActionResult> UserProfile()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var user =await _userManager.FindByIdAsync(userId);

            List<BookingListViewModel> BookedFlat = (
                            from flatBooked in _context.FlatBookeds
                            join flat in _context.Flats on flatBooked.FlatId equals flat.FlatId
                            where flatBooked.UserId == userId
                            select new BookingListViewModel
                            {
                                FlatBookedId = flatBooked.FlatBookedId,
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
            var flatsByOwner = _context.Flats.Where(f => f.IsBooking == 0 && f.OwnerId == userId).ToList();
            var flats = _context.Flats.ToList();

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
                    FlatBookedId = item.FlatBooked.FlatBookedId,
                    Picture = item.Flat.Picture,
                    UserName = user12.Name,
                    Address = user12.Address,
                    Type = user12.PhoneNumber,
                    Cost = item.Flat.TotalCost,
                    Date = item.FlatBooked.BookingDate
                });
                // Now you can use the bookingViewModel or add it to a list if needed
            }

            ViewBag.Flats = flatsByOwner;
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

        public IActionResult Enquery()
        {
            var enqueries = _context.Enqueries.ToList();
            ViewBag.Enquery = enqueries;
            return View();
        }

        public IActionResult BookingReport(int id)
        {
            string Header = "";
            var bookedList = _context.FlatBookeds.Where(b => b.FlatBookedId == id).ToList();
            var flatList = _context.Flats.ToList();
            var userList = _userManager.Users;
            JoinedFlatBookingData joinedData = new JoinedFlatBookingData();

            var query = from booking in bookedList
                        join flat in flatList on booking.FlatId equals flat.FlatId
                        join user in userList on booking.OwnerId equals user.Id
                        join user1 in userList on booking.UserId equals user1.Id
                        select new JoinedFlatBookingData
                        {
                            FlatName = flat.Name,
                            Address = $"H: {flat.HouseNo} R: {flat.RoadNo} S: {flat.sectorNo}, {flat.AreaName}",
                            Type = flat.Types.ToString(),
                            OwnerName = user.Name,
                            OwnerPhone = user.PhoneNumber,
                            BuyerName = user1.Name,
                            BuyerPhone = user1.PhoneNumber,
                            BookingDate = booking.BookingDate,
                            FlatCost = booking.FlatCost,
                        };
            joinedData = query.FirstOrDefault();
            return File(GetPDFFileForInvoice(joinedData, Header), "application/pdf");
        }

        public byte[] GetPDFFileForInvoice(JoinedFlatBookingData joinedData, string Header)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PDF Report"
            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = InvoiceHtmlTemplate.GetHtml(joinedData, Header),
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(wwwRootPath, "css", "invoice.css") },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Flat Finding" }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            byte[] file = _converter.Convert(pdf);
            return file;
        }
    }
}
