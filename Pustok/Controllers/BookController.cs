using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok.Models;
using Pustok.ViewModels;
using System.Collections.Generic;

namespace Pustok.Controllers
{
    public class BookController : Controller
    {
        private readonly PustokContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BookController(PustokContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AddToBasket(int bookId)
        {
            if (!_context.Books.Any(x => x.Id == bookId)) return NotFound();
            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            BasketItemViewModel basketItem = null;

            AppUser appUser = await Checklogin();
            if (appUser is null)
            {
                string basketItemsStr = HttpContext.Request.Cookies["BasketItems"];
                if (basketItemsStr is not null)
                {
                    basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemsStr);

                    basketItem = basketItems.FirstOrDefault(x => x.BookId == bookId);
                    if (basketItem is not null)
                    {
                        basketItem.Count++;
                    }
                    else
                    {
                        basketItem = new BasketItemViewModel
                        {
                            BookId = bookId,
                            Count = 1
                        };
                        basketItems.Add(basketItem);
                    }
                }
                else
                {
                    basketItem = new BasketItemViewModel
                    {
                        BookId = bookId,
                        Count = 1
                    };
                    basketItems.Add(basketItem);
                }
                basketItemsStr = JsonConvert.SerializeObject(basketItems);
                HttpContext.Response.Cookies.Append("BasketItems", basketItemsStr);
            }
            else
            {
                BasketItem memberBasketItem = _context.BasketItems.FirstOrDefault(x => x.AppUserId == appUser.Id && x.BookId == bookId && x.IsDeleted == false);
                if (memberBasketItem is not null)
                {
                    memberBasketItem.Count++;
                }
                else
                {
                    memberBasketItem = new BasketItem
                    {
                        BookId = bookId,
                        AppUserId = appUser.Id,
                        Count = 1,
                        IsDeleted = false
                    };
                    _context.BasketItems.Add(memberBasketItem);
                }
            }
                await _context.SaveChangesAsync();
            return Ok();
        }

        public IActionResult GetBasketItems()
        {
            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            string basketItemsStr = HttpContext.Request.Cookies["BasketItems"];

            if (basketItemsStr != null)
            {
                basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemsStr);
            }

            return Json(basketItems);
        }


        public IActionResult RemoveFromBasket(int bookId)
        {
            if (!_context.Books.Any(x => x.Id == bookId)) return NotFound();
            List<BasketItemViewModel> basketItemsVM = new List<BasketItemViewModel>();
            List<BasketItem> basketItems = null;
            

            if (!User.Identity.IsAuthenticated)
            {
                BasketItemViewModel basketItem = null;
                string basketItemsStr = HttpContext.Request.Cookies["BasketItems"];
                if (basketItemsStr != null)
                {
                    basketItemsVM = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemsStr);

                    basketItem = basketItemsVM.FirstOrDefault(x => x.BookId == bookId);
                    if (basketItem != null)
                    {
                        if (basketItem.Count > 1)
                        {
                            basketItem.Count--;
                        }
                        else
                        {
                            basketItemsVM.Remove(basketItem);
                        }
                    }
                }
                basketItemsStr = JsonConvert.SerializeObject(basketItemsVM);
                HttpContext.Response.Cookies.Append("BasketItems", basketItemsStr);
            }
            else
            {
                BasketItem basketItem = null;
                basketItems = _context.BasketItems.Include(x=> x.Book).ToList();
                basketItem = basketItems.FirstOrDefault(x => x.BookId == bookId);
                if (basketItem is not null)
                {
                    if (basketItem.Count > 1)
                    {
                        basketItem.Count--;
                    }
                    else
                    {
                        _context.BasketItems.Remove(basketItem);
                    }
                }
                _context.SaveChanges();
            }
            return RedirectToAction("Checkout", "Book");
        }

        public async Task<IActionResult> Checkout()
        {
            List<BasketItemViewModel> basketItemsVM = new List<BasketItemViewModel>();
            List<CheckoutViewModel> checkoutItems = new List<CheckoutViewModel>();
            List<BasketItem> basketItems = null;
            Order order = new Order();
            order.OrderStatus = Enums.OrderStatus.Pending;

            CheckoutViewModel checkoutItem = null;
            if (!User.Identity.IsAuthenticated)
            {
                string basketItemsStr = HttpContext.Request.Cookies["BasketItems"];
                if (basketItemsStr != null)
                {
                    basketItemsVM = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemsStr);

                    foreach (var item in basketItemsVM)
                    {
                        checkoutItem = new CheckoutViewModel
                        {
                            Book = _context.Books.FirstOrDefault(x => x.Id == item.BookId),
                            Count = item.Count
                        };
                        checkoutItems.Add(checkoutItem);
                    }

                }
            }
            else
            {
                AppUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                order.AppUserId = user.Id;
                basketItems = _context.BasketItems.Include(x => x.Book).Where(x => x.AppUserId == user.Id && x.IsDeleted == false).ToList();
                foreach (var item in basketItems)
                {

                    if (!item.IsDeleted)
                    {
						checkoutItem = new CheckoutViewModel
						{
							Book = item.Book,
							Count = item.Count
						};
						checkoutItems.Add(checkoutItem);
					}
					
                }

            }
            order.OrderItems = new List<OrderItem>();
            foreach (var item in checkoutItems)
            {
                OrderItem orderItem = null;
                
                orderItem = new OrderItem
                {
                    Book = item.Book,
                    Count = item.Count,
                    IsDeleted = false,
                    
                };

				order.OrderItems.Add(orderItem);
            }
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            List<BasketItemViewModel> basketItemsVM = new List<BasketItemViewModel>();
            List<CheckoutViewModel> checkoutItems = new List<CheckoutViewModel>();
            List<BasketItem> basketItems = null;

            CheckoutViewModel checkoutItem = null;
            if (!User.Identity.IsAuthenticated)
            {
                string basketItemsStr = HttpContext.Request.Cookies["BasketItems"];
                if (basketItemsStr != null)
                {
                    basketItemsVM = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemsStr);

                    foreach (var item in basketItemsVM)
                    {
                        checkoutItem = new CheckoutViewModel
                        {
                            Book = _context.Books.FirstOrDefault(x => x.Id == item.BookId),
                            Count = item.Count
                        };
                        checkoutItems.Add(checkoutItem);
                    }

                }
            }
            else
            {
				AppUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
				order.AppUserId = user.Id;
				basketItems = _context.BasketItems.Include(x => x.Book).Where(x => x.IsDeleted == false).ToList();
                foreach (var item in basketItems)
                {
                    item.IsDeleted = true;
                    checkoutItem = new CheckoutViewModel
                    {
                        Book = item.Book,
                        Count = item.Count
                    };
                    checkoutItems.Add(checkoutItem);
                }

            }
            order.IsDeleted= false;
            order.CreatedTime = DateTime.Now;
            double totalPrice = 0;
			order.OrderItems = new List<OrderItem>();
			foreach (var item in checkoutItems)
			{
				OrderItem orderItem = null;
                
				orderItem = new OrderItem
				{
					Book = item.Book,
					Count = item.Count,
					IsDeleted = false,
                    Order = order,

                    
				};
                totalPrice += (item.Book.SalePrice * (1 - (item.Book.DiscountPrice / 100))) * item.Count;

                order.OrderItems.Add(orderItem);
                _context.OrderItems.Add(orderItem);

            }
            order.TotalPrice = totalPrice;
            if (!ModelState.IsValid)
            {
                return View(order);
            }
            _context.Orders.Add(order);
            _context.SaveChanges();
            return RedirectToAction("index" ,"home");
		}

        public async Task<AppUser> Checklogin()
        {
            bool check = HttpContext.User.Identity.IsAuthenticated;
            if (check)
            {
                AppUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                return user;
            }
            return null;
        }
    }
}

