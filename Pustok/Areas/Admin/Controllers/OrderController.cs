using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Models;

namespace Pustok.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly PustokContext _context;

        public OrderController( PustokContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Order> orders = _context.Orders.ToList();
            return View(orders);
        }

        public IActionResult Detail(int id)
        {
            Order order = _context.Orders.Include(x => x.OrderItems).FirstOrDefault(x => x.Id == id);
            if (order == null) return NotFound();

            return View(order);
        }

        public IActionResult Accept(int id)
        {
            Order order= _context.Orders.FirstOrDefault(x=> x.Id == id);
            order.OrderStatus = Enums.OrderStatus.Accepted;
            _context.SaveChanges();
            return RedirectToAction("Index");   
        }

        public IActionResult Reject(int id)
        {
            Order order= _context.Orders.FirstOrDefault(x => x.Id == id);
            order.OrderStatus = Enums.OrderStatus.Rejected;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
