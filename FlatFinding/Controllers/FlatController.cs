using FlatFinding.Data;
using FlatFinding.Migrations;
using FlatFinding.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FlatFinding.ViewModel;

namespace FlatFinding.Controllers
{
    [Authorize]
    public class FlatController : Controller
    {
        private readonly FlatFindingContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public FlatController(UserManager<ApplicationUser> userManager, FlatFindingContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var AllFlats = await _context.Flats
                .Where(f => f.IsBooking == 0 && f.Available == "YES" && f.FlatStatus == "Accepted").ToListAsync();
            ViewBag.Flats = AllFlats;
            ViewBag.Count = AllFlats.Count;
            return View();
        }
        
        public async Task<IActionResult> AreaWise(string id)
        {
            var AllFlats = await _context.Flats 
                .Where(f => f.IsBooking == 0 && f.Available == "YES" && f.FlatStatus == "Accepted").ToListAsync();

            AllFlats = AllFlats.Where(flat => flat.AreaName == id).ToList();

            ViewBag.Flats = AllFlats;
            ViewBag.Count = AllFlats.Count;
            return View("Index");
        }

        public async Task<IActionResult> Search(SearchViewModel model)
        {
            var AllFlats = await _context.Flats
                .Where(f => f.IsBooking == 0 && f.Available == "YES" && f.FlatStatus == "Accepted").ToListAsync();

            AllFlats = AllFlats.Where(flat => (flat.AreaName == model.Area) && (flat.Types == model.Type)).ToList();
            AllFlats = AllFlats.Where(flat => (flat.TotalCost >= (model.Price - 3000) && flat.TotalCost <= (model.Price + 2000))).ToList();
            AllFlats = AllFlats.Where(flat => (flat.RoomNo >= (model.Room - 2) && flat.RoomNo <= (model.Room + 2))).ToList();


            ViewBag.Flats = AllFlats;
            ViewBag.Count = AllFlats.Count;
            return View("Index");
        }

        public async Task<IActionResult> FlatDetails(int? id, int IsNotBooking = 0)
        {
            var FlatDetail = await _context.Flats
                .FirstOrDefaultAsync(m => m.FlatId == id);
            var comments = await _context.Comments
                .Where(f => f.FlatId == id).ToListAsync();

            if (FlatDetail == null) 
                return NotFound();
            
            

            FlatDetail.Views = FlatDetail.Views + 1;

            _context.Update(FlatDetail);
            await _context.SaveChangesAsync();

            ViewBag.FlatDetail = FlatDetail;
            ViewBag.Comments = comments;

            if(IsNotBooking == 1)
            {
                TempData["flat"] = "You can Not Booked more then 5 Flats";
            }
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(Flat model, IFormFile? file)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            model.OwnerId = userId;

            // If search on flat and check same flat entry or not
            if (ModelState.IsValid)
            {
                var flatExist = await _context.Flats.Where(
                f => f.FlatNumber == model.FlatNumber
               && f.RoadNo== model.RoadNo && f.HouseNo== model.HouseNo
               && f.sectorNo == model.sectorNo && f.AreaName== model.AreaName
               ).ToListAsync();

            if(flatExist.Count > 0)
            {
                ViewBag.SameFlat = "Same";
            }
            else
            {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    if (file != null)
                    {
                        string fileName = Guid.NewGuid().ToString();
                        var uploads = Path.Combine(wwwRootPath, @"img");
                        var extension = Path.GetExtension(file.FileName);

                        if (model.Picture != null)
                        {
                            var oldImagePath = Path.Combine(wwwRootPath, model.Picture.TrimStart('\\'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }


                        using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                        {
                            file.CopyTo(fileStreams);
                        }

                        model.Picture = @"\img\" + fileName + extension;

                    };

                    model.FlatStatus = "Pending";

                    _context.Add(model);
                    await _context.SaveChangesAsync();
                    TempData["flat"] = "Flat Save Successfully";
                    return RedirectToAction("FlatOwnerProfile", "Dashboard");
                }
            

           
               
            }
            TempData["flat"] = "Flat Not Save";
            return View( model);
        }

        [HttpGet]
        public IActionResult UpdateFlat(int id)
        {
            var model = _context.Flats.FirstOrDefault(x => x.FlatId == id);

            ViewBag.Picture = model.Picture;
            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateFlat(Flat model, IFormFile? file)
        {
           
          

            var flatExist =  _context.Flats.Where(
            f => f.FlatNumber == model.FlatNumber
           && f.RoadNo == model.RoadNo && f.HouseNo == model.HouseNo
           && f.sectorNo == model.sectorNo && f.AreaName == model.AreaName
           && f.Types == model.Types && f.Phone ==  model.Phone 
           && f.Description == model.Description && f.IsBooking == 0  
           ).ToList();

            if (flatExist.Count > 0)
            {
                ViewBag.SameFlat = "Same";
            }
            else
            {
                var flat = _context.Flats.AsNoTracking().FirstOrDefault(f => f.FlatId == model.FlatId);
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"img");
                    var extension = Path.GetExtension(file.FileName);

                    if (model.Picture != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, model.Picture.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }


                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }

                    flat.Picture = @"\img\" + fileName + extension;

                };


                model.Picture = flat.Picture;
                model.OwnerId = flat.OwnerId;

                _context.Flats.Update(model);
                _context.SaveChanges();
                return RedirectToAction("UpdateFlat", new { id = model.FlatId });
            }

           
            
            return View(model);
        }
        public async Task<IActionResult> DeleteFlat(int id)
        {
            var userModel = await _context.Flats.FindAsync(id);
            if (userModel != null)
            {
                _context.Flats.Remove(userModel);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("FlatOwnerProfile", "Dashboard");
        }


        [HttpPost]
        public async Task<IActionResult> Comment(int FlatId, string comment)
        {
            try
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                var user = await _userManager.FindByIdAsync(userId);
                Comment comt = new Comment()
                {
                    FlatId = FlatId,
                    comment = comment,
                    Date = DateTime.Now,
                    Name = user.Name,
                };

                _context.Comments.Add(comt);
                await _context.SaveChangesAsync();
            }
            catch(Exception)
            {
                
            }


            return RedirectToAction("FlatDetails", new { id = FlatId });
        }
    }
}
