using FlatFinding.Data;
using FlatFinding.Models;
using FlatFinding.Models.PaymentGetway;
using FlatFinding.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;
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
            var baseUrl = Request.Scheme + "://" + Request.Host;

            // CREATING LIST OF POST DATA
            NameValueCollection PostData = new NameValueCollection();

            PostData.Add("total_amount", $"{FlatDetail.TotalCost}");
            PostData.Add("tran_id", "TESTASPNET1234");
            PostData.Add("success_url", baseUrl + "/Payment/PaymentGetWay?id="+ id);
            PostData.Add("fail_url", baseUrl + "/Payment/CheckoutFail");
            PostData.Add("cancel_url", baseUrl + "/Payment/CheckoutCancel");

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

       
        public async Task<IActionResult> PaymentGetWay(int id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            // If All okay then booked the flat
            // and make flat avilable NO
            // All detail info Pdf shwo to user for download 
            // if success other wise only confirm Message Tost Notification 

            // comission 1000 + (5)


            var FlatDetail = await _context.Flats
                .FirstOrDefaultAsync(m => m.FlatId == id);
            if (FlatDetail == null)
                return NotFound();

            FlatBooked flatBooked = new FlatBooked()
            {
                FlatId = id,
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
            return RedirectToAction("Index", "Home");
        }
    }
}
