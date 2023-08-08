using FlatFinding.Data;
using FlatFinding.Migrations;
using FlatFinding.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> FlatDetails(int? id)
        {
            var FlatDetail = await _context.Flats
                .FirstOrDefaultAsync(m => m.FlatId == id);

            if (FlatDetail == null) 
                return NotFound();
            
            

            FlatDetail.Views = FlatDetail.Views + 1;

            _context.Update(FlatDetail);
            await _context.SaveChangesAsync();

            ViewBag.FlatDetail = FlatDetail;

            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Update(Flat model, IFormFile? file)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            model.OwnerId = userId;

            if (ModelState.IsValid)
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

                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}
