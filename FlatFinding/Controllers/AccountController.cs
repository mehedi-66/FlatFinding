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

        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, 
                                    SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        

        [HttpGet]
        public IActionResult Register()
        {
            var roles = roleManager.Roles;
            ViewBag.Roles = roles;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.RoleId);
            if(role == null)
            {
                return View(model);
            }

            if(ModelState.IsValid)
            {
                var user = new ApplicationUser() { Email = model.Email, UserName = model.Email,
                    Address = model.Address, PhoneNumber = model.PhoneNumber, Name = model.Name
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
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            if (ModelState.IsValid)
            {

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password,
                                                                    model.RememberMe, false);

                if (result.Succeeded)
                {
                    if(! string.IsNullOrEmpty(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                   
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
