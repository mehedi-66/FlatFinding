using DinkToPdf;
using DinkToPdf.Contracts;
using FlatFinding.Data;
using FlatFinding.ReportTemplate;
using Microsoft.AspNetCore.Mvc;

namespace FlatFinding.Controllers
{
    public class ReportController : Controller
    {
        private IConverter _converter;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly FlatFindingContext _context;
        public ReportController(FlatFindingContext context, IConverter converter, IWebHostEnvironment webHostEnvironment)
        {
            _converter = converter;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AllBookingReport()
        {
           var bookedList =  _context.FlatBookeds.ToList();
            string wwwRootPath = _webHostEnvironment.WebRootPath;
           //string saveLocationOfPdf =  Path.Combine(wwwRootPath, @"pdf");
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
                HtmlContent = BookedHtmlTemplate.GetHtml(bookedList),
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(wwwRootPath, "css", "report.css") },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf");
        }
    }
}
