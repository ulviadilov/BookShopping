using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Models;
using Pustok.ViewModels;

namespace Pustok.Controllers
{
    public class AccauntController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly PustokContext _context;

        public AccauntController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, PustokContext context )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(MemberRegisterViewModel memberRegisterVM)
        {
            if(!ModelState.IsValid) return View();

            AppUser user = null;
            user = await _userManager.FindByNameAsync(memberRegisterVM.Username);
            if (user is not null)
            {
                ModelState.AddModelError("Username" , "Already exist");
                return View();
            }

            user = await _userManager.FindByEmailAsync(memberRegisterVM.Email);
            if (user is not null)
            {
                ModelState.AddModelError("Email", "Already exist");
                return View();
            }
            user = new AppUser
            {
                Fullname = memberRegisterVM.Fullname,
                Email = memberRegisterVM.Email,
                UserName = memberRegisterVM.Username,
                IsAdmin = false
            };

            var result = await _userManager.CreateAsync(user,memberRegisterVM.Password);
            if(result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("",err.Description);
                }
            }
            await _userManager.AddToRoleAsync(user,"Member");
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Login","Accaunt");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginViewModel userLoginVM)
        {
            if(!ModelState.IsValid) return View();
            AppUser user = await  _userManager.FindByNameAsync(userLoginVM.Username);
            if (user is null )
            {
                ModelState.AddModelError("" , "Username or password incorrect");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user,userLoginVM.Password,false,false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or password incorrect");
                return View();
            }
            return RedirectToAction("Index","Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("login" , "accaunt");
        }

        public async Task<IActionResult> Profile()
        {
            AppUser user = null;
            if(User.Identity.IsAuthenticated)
            {
                user =await _userManager.FindByNameAsync(User.Identity.Name);
            }
            List<Order> orders = _context.Orders.Where(x => x.AppUserId == user.Id).ToList();

            return View(orders);
        }
    }
}
