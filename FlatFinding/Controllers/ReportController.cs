
using DinkToPdf;
using DinkToPdf.Contracts;
using FlatFinding.Data;
using FlatFinding.Models;
using FlatFinding.ReportTemplate;
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
            }
            else if(id == 2)
            {
                //Custom Date Range
            }
            else if(id == 3)
            {
                //Custom Month
            }
            
           return View();
            
           
        }

        [HttpGet]
        public IActionResult AllBookingReport(int id)
        {
            string Header = "All";
            var bookedList = _context.FlatBookeds.ToList();
            var flatList = _context.Flats.ToList();
            var userList = _userManager.Users.ToList();
            List<JoinedFlatBookingData> joinedData = new List<JoinedFlatBookingData>();
            /* 
             1) Todays
            2)  Last 7 Days
            3)  Last Month
            4)  Generate All Booking
             
             */
            if(id == 1)
            { // Todays Booking  
                Header = "Todays";
            }
            else if(id == 2)
            { // Last 7 days  
                Header = "Last 7 Days";
            }
            else if(id == 3)
            { // Last month 
                Header = "Last Month";
            }

            var query = from booking in bookedList
                        join flat in flatList on booking.FlatId equals flat.FlatId
                        join user in userList on flat.OwnerId equals user.Id
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
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
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
