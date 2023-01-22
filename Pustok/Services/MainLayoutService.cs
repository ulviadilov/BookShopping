using Microsoft.AspNetCore.Identity;
using Pustok.Models;

namespace Pustok.Services
{
    public class MainLayoutService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public MainLayoutService(UserManager<AppUser> userManager, IHttpContextAccessor httpContext)
        {
            _userManager = userManager;
            _httpContext = httpContext;
        }
        public async Task<AppUser> GetUser()
        {
            string name = _httpContext.HttpContext.User.Identity.Name;
            AppUser user = await _userManager.FindByNameAsync(name);
            return user;
        }
    }
}
