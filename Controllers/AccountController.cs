using System.Threading.Tasks;
using LogInWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LogInWeb.Controllers
{
    public class AccountController:Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManage;
        public AccountController(UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManage)
        {
            _userManager=userManager;
            _signInManage=signInManage;
        }
        [AllowAnonymous]
        
        public IActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel model,string returnUrl)
        {
            if(ModelState.IsValid)
            {
                var result=await _signInManage.PasswordSignInAsync(model.Username, model.Password,model.RememberMe,false);
                if(result.Succeeded&&Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }  
                ModelState.AddModelError("","Login Invalid");       
            }
            return View(model);
        }
        [HttpPost]

        public async Task<IActionResult> Logout()
        {
            await _signInManage.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if(ModelState.IsValid)
            {
                var user=new AppUser{
                    UserName=model.Username,
                    Email=model.Email
                };
                var result = await _userManager.CreateAsync(user,model.Password);
                if(result.Succeeded)
                {
                    if(_signInManage.IsSignedIn(User)&&User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers","Administration");
                    }
                    await _signInManage.SignInAsync(user,isPersistent:false);
                    return RedirectToAction("Index","Home");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }
            }
            return View(model);
        }
        public IActionResult AccessDenied()
        {
            return View("Denied");
        }

    }
}