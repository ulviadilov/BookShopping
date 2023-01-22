using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pustok.Models;

namespace Pustok.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "SuperAdmin ,Ulvi")]
	public class DashboardController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DashboardController(UserManager<AppUser> userManager , RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
            _roleManager = roleManager;
        }

		public IActionResult Index()
		{
			return View();
		}
		public async Task<IActionResult> CreateAdmin()
		{
			AppUser admin = new AppUser
			{
				UserName = "Ulvi",
				Fullname = "Ulvi Adilov"
			};

			var result = await _userManager.CreateAsync(admin, "Adilov23");
			return Ok(result);
		}

		//public async Task<IActionResult> CreateRole()
		//{
		//	IdentityRole role1 = new IdentityRole("SuperAdmin");
		//	IdentityRole role2 = new IdentityRole("SuperAdmin");
		//	IdentityRole role3 = new IdentityRole("Member");
		//	await _roleManager.CreateAsync(role1);
		//	await _roleManager.CreateAsync(role2);
		//	await _roleManager.CreateAsync(role3);

		//	return Ok("Created");
		//}

		//public async Task<IActionResult> AddRole()
		//{
		//	AppUser user = await _userManager.FindByNameAsync("Ulvi");

		//	await _userManager.AddToRoleAsync(user, "SuperAdmin");
		//	return Ok("Role added");
		//}

	}
}
