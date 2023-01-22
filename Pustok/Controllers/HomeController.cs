using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Models;
using Pustok.ViewModels;
using System.Diagnostics;

namespace Pustok.Controllers
{
    public class HomeController : Controller
    {
        private readonly PustokContext _context;

        public HomeController(PustokContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            HomeViewModel HomeVm = new HomeViewModel
            {
                Sliders = _context.Sliders.ToList(),
                FeaturedBooks = _context.Books.Include(x => x.Author).Include(x => x.BookImages).Where(x => x.IsFeatured).ToList(),
                NewBooks = _context.Books.Include(x => x.Author).Include(x => x.BookImages).Where(x => x.IsNew).ToList(),
                DiscountedBooks = _context.Books.Include(x => x.Author).Include(x => x.BookImages).Where(x => x.DiscountPrice > 0).ToList()
            };
            return View(HomeVm);
        }

        public IActionResult Details(int id)
        {
            Book book = _context.Books.Include(x => x.Category).Include(x => x.Author).Include(x => x.BookImages).FirstOrDefault(x => x.Id == id);
            return View(book);
        }

    }
}