using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pustok.Areas.Admin.Services;
using Pustok.Models;
using Pustok.Services;

namespace Pustok
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<PustokContext>(opt =>
            {
                opt.UseSqlServer("Server=CHIEF\\SQLEXPRESS;Database=PustokDB;Trusted_Connection=True");
            });

            builder.Services.AddIdentity<AppUser , IdentityRole>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireDigit = true;
                opt.Password.RequiredUniqueChars = 0;
                opt.Password.RequiredLength = 8;

                opt.User.RequireUniqueEmail = false;

            }).AddEntityFrameworkStores<PustokContext>().AddDefaultTokenProviders() ;
            builder.Services.AddScoped<LayoutService>();
            builder.Services.AddScoped<MainLayoutService>();
            var app = builder.Build();

            

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
          );

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}