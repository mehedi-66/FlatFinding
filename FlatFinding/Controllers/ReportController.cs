
using DinkToPdf;
using DinkToPdf.Contracts;
using FlatFinding.Data;
using FlatFinding.Models;
using FlatFinding.ReportTemplate;
using FlatFinding.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FlatFinding.Controllers
{
    public class ReportController : Controller
    {
        private IConverter _converter;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly FlatFindingContext _context;
        public ReportController(UserManager<ApplicationUser> userManager, FlatFindingContext context, IConverter converter, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _converter = converter;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }
        public IActionResult Index(int id = 0)
        {
           
            if(id == 1)
            {
                //Custom Date
                ViewBag.CustomDate = 1;
            }
            else if(id == 2)
            {
                //Custom Date Range
                ViewBag.CustomDateRange = 2;
            }
            else if(id == 3)
            {
                //Custom Month
                ViewBag.CustomMonth = 3;
            }
            
           return View();
            
           
        }

        public IActionResult Custom(CustomDateViewModel model)
        {
            string Header = "All";
            var bookedList = _context.FlatBookeds.ToList();
            var flatList = _context.Flats.ToList();
            var userList = _userManager.Users.ToList();
            List<JoinedFlatBookingData> joinedData = new List<JoinedFlatBookingData>();

            if (model.Id == 1 && model.StartDate != null)
            {
                // date 
                Header = "Custom Date";

                bookedList = bookedList.Where(booking => booking.BookingDate.Date == model.StartDate?.Date).ToList();
            }
            else if(model.Id == 3 && model.StartDate != null)
            {
                // month
                
                Header = "Custom Month";
                bookedList = bookedList.Where(booking =>
                                            booking.BookingDate.Year == model.StartDate?.Date.Year &&
                                            booking.BookingDate.Month == model.StartDate?.Date.Month
                                            ).ToList();
            }
            else if(model.Id == 2 && model.StartDate != null && model.EndDate != null)
            {
                // date range
                Header = "Custom Date Range";
                bookedList = bookedList.Where(booking =>
                                            booking.BookingDate.Date >= model.StartDate?.Date && 
                                            booking.BookingDate.Date <= model.EndDate?.Date
                                        ).ToList();
            }
            else
            {
                return RedirectToAction("Index");
            }

            var query = from booking in bookedList
                        join flat in flatList on booking.FlatId equals flat.FlatId
                        join user in userList on booking.OwnerId equals user.Id
                        join user1 in userList on booking.UserId equals user1.Id
                        select new JoinedFlatBookingData
                        {
                            FlatName = flat.Name,
                            Address = $"H: {flat.HouseNo} R: {flat.RoadNo} S: {flat.sectorNo}, {flat.AreaName}",
                            OwnerName = user.Name,
                            BuyerName = user1.Name,
                            BookingDate = booking.BookingDate,
                            FlatCost = booking.FlatCost,
                            FlatProfit = booking.FlatProfit
                        };

            joinedData = query.ToList();

            return File(GetPDFFile(joinedData, Header), "application/pdf");
        }

        [HttpGet]
        public IActionResult AllBookingReport(int id)
        {
            string Header = "All";
            var bookedList = _context.FlatBookeds.ToList();
            var flatList = _context.Flats.ToList();
            var userList = _userManager.Users.ToList();
            List<JoinedFlatBookingData> joinedData = new List<JoinedFlatBookingData>();
          
            if(id == 1)
            { // Todays Booking  
                Header = "Todays";
                DateTime today = DateTime.Today;

                bookedList = bookedList.Where(booking => booking.BookingDate.Date == today).ToList();
            }
            else if(id == 2)
            { // Last 7 days  
                Header = "Last 7 Days";
                DateTime lastWeek = DateTime.Today.AddDays(-7);

                bookedList = bookedList.Where(booking => booking.BookingDate >= lastWeek).ToList();
            }
            else if(id == 3)
            { // Last month 
                Header = "Last Month";
                DateTime today = DateTime.Today;
                DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
                DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                bookedList = bookedList.Where(booking => booking.BookingDate >= firstDayOfMonth && booking.BookingDate <= lastDayOfMonth).ToList();
            }

            var query = from booking in bookedList
                        join flat in flatList on booking.FlatId equals flat.FlatId
                        join user in userList on booking.OwnerId equals user.Id
                        join user1 in userList on booking.UserId equals user1.Id
                        select new JoinedFlatBookingData
                        {
                            FlatName = flat.Name,
                            Address = $"H: {flat.HouseNo} R: {flat.RoadNo} S: {flat.sectorNo}, {flat.AreaName}",
                            OwnerName = user.Name,
                            BuyerName = user1.Name,
                            BookingDate = booking.BookingDate,
                            FlatCost = booking.FlatCost,
                            FlatProfit = booking.FlatProfit
                        };

            joinedData = query.ToList();



           
            return File(GetPDFFile(joinedData, Header), "application/pdf");
        }

        public byte[] GetPDFFile(List<JoinedFlatBookingData> joinedData, string Header)
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
                HtmlContent = BookedHtmlTemplate.GetHtml(joinedData, Header),
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(wwwRootPath, "css", "report.css") },
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
