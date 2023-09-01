using DinkToPdf;
using DinkToPdf.Contracts;
using FlatFinding.Data;
using FlatFinding.Models;
using FlatFinding.Models.PaymentGetway;
using FlatFinding.ReportTemplate;
using FlatFinding.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Collections.Specialized;
using System.Data;

namespace FlatFinding.Controllers
{
    [Authorize(Roles = "User")]
    public class PaymentController : Controller
    {
       
        private IConverter _converter;
        private readonly FlatFindingContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PaymentController(IConverter converter, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager, FlatFindingContext context)
        {
            _context = context;
            _userManager = userManager;
            _converter = converter;
            _webHostEnvironment = webHostEnvironment;

        }
        public async Task<IActionResult> PoliceVerify(int? id)
        {
            ViewBag.Id = id;
            return View();
        }
        public async Task<IActionResult> Index(FlatBooked model, IFormFile? file)
        {
            int id = model.FlatId;
            var UserId = _userManager.GetUserId(HttpContext.User);
            HttpContext.Session.SetString("LoggedIn", UserId);

            var flatBookingByuser = await _context.FlatBookeds.Where(f => f.IsDelete == 0 && f.UserId == UserId).ToListAsync();
            if(flatBookingByuser.Count < 5)
            {

                var FlatDetail = await _context.Flats
                    .FirstOrDefaultAsync(m => m.FlatId == id);
                if (FlatDetail == null)
                    return NotFound();

                ViewBag.Cost = FlatDetail.TotalCost;
                ViewBag.FlatId = id;
                var baseUrl = Request.Scheme + "://" + Request.Host;
                /* var baseUrl = "https://localhost:7168/";*/

                // CREATING LIST OF POST DATA
                NameValueCollection PostData = new NameValueCollection();

                PostData.Add("total_amount", $"{FlatDetail.TotalCost}");
                PostData.Add("tran_id", "TESTASPNET1234");
                PostData.Add("success_url", baseUrl + "/Payment/PaymentGetWay?id=" + id);
                PostData.Add("fail_url", baseUrl + "/Flat/FlatDetails?id=" + id);
                PostData.Add("cancel_url", baseUrl + "/Flat/FlatDetails?id=" + id);

                PostData.Add("version", "3.00");
                PostData.Add("cus_name", "ABC XY");
                PostData.Add("cus_email", "abc.xyz@mail.co");
                PostData.Add("cus_add1", "Address Line On");
                PostData.Add("cus_add2", "Address Line Tw");
                PostData.Add("cus_city", "City Nam");
                PostData.Add("cus_state", "State Nam");
                PostData.Add("cus_postcode", "Post Cod");
                PostData.Add("cus_country", "Countr");
                PostData.Add("cus_phone", "0111111111");
                PostData.Add("cus_fax", "0171111111");
                PostData.Add("ship_name", "ABC XY");
                PostData.Add("ship_add1", "Address Line On");
                PostData.Add("ship_add2", "Address Line Tw");
                PostData.Add("ship_city", "City Nam");
                PostData.Add("ship_state", "State Nam");
                PostData.Add("ship_postcode", "Post Cod");
                PostData.Add("ship_country", "Countr");
                PostData.Add("value_a", "ref00");
                PostData.Add("value_b", "ref00");
                PostData.Add("value_c", "ref00");
                PostData.Add("value_d", "ref00");
                PostData.Add("shipping_method", "NO");
                PostData.Add("num_of_item", "1");
                PostData.Add("product_name", $"{FlatDetail.Name}");
                PostData.Add("product_profile", "general");
                PostData.Add("product_category", "Demo");

                //we can get from email notificaton
                var storeId = "gsa64e872df83b0e";
                var storePassword = "gsa64e872df83b0e@ssl";
                var isSandboxMood = true;

                SSLCommerzGatewayProcessor sslcz = new SSLCommerzGatewayProcessor(storeId, storePassword, isSandboxMood);

                string response = sslcz.InitiateTransaction(PostData);

                return Redirect(response);
            }
            else
            {
                return RedirectToAction("FlatDetails", "Flat", new { id = id, IsNotBooking = 1 }) ;
            }
            

        }

       
        public async Task<IActionResult> PaymentGetWay(int id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var userId2 = HttpContext.Session.GetString("LoggedIn");

            if(userId == userId2)
            {
                var FlatDetail = await _context.Flats
               .FirstOrDefaultAsync(m => m.FlatId == id);
                if (FlatDetail == null)
                    return NotFound();

                if (FlatDetail.IsBooking == 1)
                    return RedirectToAction("UserProfile", "Dashboard");

                FlatBooked flatBooked = new FlatBooked()
                {
                    FlatId = id,
                    OwnerId = FlatDetail.OwnerId,
                    UserId = userId,
                    FlatCost = FlatDetail.TotalCost,
                    FlatProfit = (FlatDetail.TotalCost / 1000) + 20,
                    BookingDate = DateTime.Now,
                    PaymentId = new Random().Next(int.MinValue, int.MaxValue).ToString(),
                };

                // Save Booked 
                _context.Add(flatBooked);
                await _context.SaveChangesAsync();

                // Update Flat Info
                FlatDetail.IsBooking = 1;
                _context.Update(FlatDetail);
                await _context.SaveChangesAsync();


            }
            else
            {
                // Refund payment 
                return RedirectToAction("FlatDetails", "Flat", new { id = id });
            }






            // Invoice Report Generate
            /* string Header = "";
             var bookedList = _context.FlatBookeds.Where(b => b.FlatId == id).ToList();
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
                             BuyerPhone= user1.PhoneNumber,
                             BookingDate = booking.BookingDate,
                             FlatCost = booking.FlatCost,
                         };
             joinedData = query.FirstOrDefault();

             // Redicent to User Profile and Report Generate Pdf 
             return File(GetPDFFileForInvoice(joinedData, Header), "application/pdf");*/

            return RedirectToAction("UserProfile", "Dashboard");

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
