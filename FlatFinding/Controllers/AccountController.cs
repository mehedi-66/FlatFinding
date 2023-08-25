using FlatFinding.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FlatFinding.Models;

namespace FlatFinding.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, 
                                    SignInManager<ApplicationUser> signInManager, IWebHostEnvironment webHostEnvironment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
        }

        

        [HttpGet]
        public IActionResult Register()
        {
            var roles = roleManager.Roles;//.Where(rol => rol.Name != "Admin").ToList();
           
            ViewBag.Roles = roles;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model, IFormFile? file)
        {
            var role = await roleManager.FindByIdAsync(model.RoleId);
            var roles = roleManager.Roles; //.Where(rol => rol.Name != "Admin").ToList();
            ViewBag.Roles = roles;
            if (role == null)
            {
                return View(model);
            }

            if(ModelState.IsValid)
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

                var user = new ApplicationUser()
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    Name = model.Name,
                    MotherName = model.MotherName,
                    FatherName = model.FatherName,
                    Picture = model.Picture

                };
                var result = await userManager.CreateAsync(user, model.Password);

                if(result.Succeeded)
                {
                    user = await userManager.FindByIdAsync(user.Id);
                    
                    await userManager.AddToRoleAsync(user, role.Name);

                   await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

          

            return View(model);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use.");
            }
        }

            [HttpGet]
        public  IActionResult Login()
        {
            var roles = roleManager.Roles;
            ViewBag.Roles = roles;
            return View();

        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            var role = await roleManager.FindByIdAsync(model.RoleId);
            var roles = roleManager.Roles;
            ViewBag.Roles = roles;

            if (role == null)
            {
                return View(model);
            }
            if (ModelState.IsValid)
            {

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password,
                                                                    model.RememberMe, false);

                if (result.Succeeded)
                {
                    if(returnUrl != null)
                    {
                        return Redirect(returnUrl);
                    }
                         
                        if(role.Name == "Admin")
                        {
                            return RedirectToAction("AdminDashboard", "Dashboard");
                        }
                        else if(role.Name == "User")
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else if(role.Name == "Owner")
                        {
                            return RedirectToAction("FlatOwnerProfile", "Dashboard");
                        }
                        return LocalRedirect(returnUrl);
                        
                    
                   
                }

            
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
             
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }


    }
}
