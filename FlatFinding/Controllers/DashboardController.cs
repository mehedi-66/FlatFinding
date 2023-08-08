using FlatFinding.Data;
using FlatFinding.Migrations;
using FlatFinding.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlatFinding.Controllers
{
    public class DashboardController : Controller
    {
        private readonly FlatFindingContext _context;
        public DashboardController(FlatFindingContext context)
        {
            _context = context;
        }
        public IActionResult UserProfile()
        {
            return View();
        } 
        
        public IActionResult FlatOwnerProfile()
        {
            return View();
        } 
        
        public IActionResult AdminDashboard()
        {
            return View();
        }

        [HttpGet]      
        public IActionResult UserList()
        {
            return View();
        }

        [HttpGet]
        public IActionResult OwnerList()
        {
            return View();
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
    }
}
