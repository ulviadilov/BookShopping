using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok.Helpers;
using Pustok.Models;
using Pustok.ViewModels;
using System.Collections.Generic;

namespace Pustok.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookController : Controller
    {
        private PustokContext _context;
        private readonly IWebHostEnvironment _env;

        public BookController(PustokContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            return View(_context.Books.ToList());
        }

        public IActionResult Create()
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Create(Book book)
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            if (!ModelState.IsValid) { return View(); }
            //multiple file
            if (book.ImageFiles != null)
            {
                foreach (IFormFile imageFile in book.ImageFiles)
                {
                    if (imageFile.ContentType != "image/jpeg" && imageFile.ContentType != "image/png")
                    {
                        ModelState.AddModelError("ImageFiles", "You can only upload png or jpeg files");
                        return View();
                    }

                    if (imageFile.Length > 2097152)
                    {
                        ModelState.AddModelError("ImageFiles", "You can only upload images under 2 mb");
                        return View();
                    }

                    BookImage bookImage = new BookImage
                    {
                        Book = book,
                        Image = imageFile.SaveFile(_env.WebRootPath, "uploads/books"),
                        IsPoster = null
                    };
                    _context.BookImages.Add(bookImage);

                }
            }

            //poster image
            if (book.PosterImageFile is not null)
            {
                if (book.PosterImageFile.ContentType != "image/jpeg" && book.PosterImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("PosterImageFile", "You can only upload png or jpeg files");
                    return View();
                }

                if (book.PosterImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("PosterImageFile", "You can only upload images under 2 mb");
                    return View();
                }
                BookImage bookImage = new BookImage
                {
                    Book = book,
                    IsPoster = true,
                    Image = book.PosterImageFile.SaveFile(_env.WebRootPath, "uploads/books")
                };
                _context.BookImages.Add(bookImage);
            }

            //hover image
            if (book.HoverImageFile is not null)
            {
                if (book.HoverImageFile.ContentType != "image/jpeg" && book.HoverImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("HoverImageFile", "You can only upload png or jpeg files");
                    return View();
                }

                if (book.HoverImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("HoverImageFile", "You can only upload images under 2 mb");
                    return View();
                }

                BookImage bookImage = new BookImage
                {
                    Book = book,
                    IsPoster = false,
                    Image = book.HoverImageFile.SaveFile(_env.WebRootPath, "uploads/books"),
                };
                _context.BookImages.Add(bookImage);
            }
            _context.Books.Add(book);
            _context.SaveChanges();
            return RedirectToAction("Index");

        }

        public IActionResult Edit(int id)
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            Book book = _context.Books.Include(x => x.BookImages).FirstOrDefault(x => x.Id == id);
            if (book == null) { return View("Error"); }
            return View(book);
        }


        [HttpPost]
        public IActionResult Edit(Book book)
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            Book existBook = _context.Books.Include(x => x.BookImages).FirstOrDefault(x => x.Id == book.Id);
            if (existBook is null) return NotFound();
            foreach (var bookImage in existBook.BookImages.FindAll(bi => !book.BookImageIds.Contains(bi.Id) && bi.IsPoster == null))
            {
                string deletepath = Path.Combine(_env.WebRootPath, "uploads/books", bookImage.Image);
                if (System.IO.File.Exists(deletepath))
                {
                    System.IO.File.Delete(deletepath);
                }
            }
            existBook.BookImages.RemoveAll(bi => !book.BookImageIds.Contains(bi.Id) && bi.IsPoster == null);

            //foreach (var posterImage in existBook.BookImages.FindAll(i => book.PosterImageId != i.Id && i.IsPoster == true))
            //{
            //    string posterDeletePath = Path.Combine(_env.WebRootPath, "uploads/books", posterImage.Image);
            //    if (System.IO.File.Exists(posterDeletePath))
            //    {
            //        System.IO.File.Delete(posterDeletePath);
            //    }
            //    existBook.BookImages.Remove(posterImage);
            //}


            //foreach (var hoverImage in existBook.BookImages.FindAll(i => book.HoverImageId != i.Id && i.IsPoster == false))
            //{
            //    string hoverDeletePath = Path.Combine(_env.WebRootPath, "uploads/books", hoverImage.Image);
            //    if (System.IO.File.Exists(hoverDeletePath))
            //    {
            //        System.IO.File.Delete(hoverDeletePath);
            //    }
            //    existBook.BookImages.Remove(hoverImage);
            //}



            if (book.PosterImageFile is not null)
            {
                if (book.PosterImageFile.ContentType != "image/jpeg" && book.PosterImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("PosterImageFile", "You can only upload png or jpeg files");
                    return View();
                }

                if (book.PosterImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("PosterImageFile", "You can only upload images under 2 mb");
                    return View();
                }
                string deletePath = Path.Combine(_env.WebRootPath, "uploads/books", existBook.BookImages.FirstOrDefault(x => x.IsPoster == true)?.Image);

                if (System.IO.File.Exists(deletePath))
                {
                    System.IO.File.Delete(deletePath);
                }
                existBook.BookImages.FirstOrDefault(x => x.IsPoster == true).Image = book.PosterImageFile.SaveFile(_env.WebRootPath, "uploads/books");
            }


            if (book.HoverImageFile is not null)
            {
                if (book.HoverImageFile.ContentType != "image/jpeg" && book.HoverImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("HoverImageFile", "You can only upload png or jpeg files");
                    return View();
                }

                if (book.HoverImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("HoverImageFile", "You can only upload images under 2 mb");
                    return View();
                }
                string deletePath = Path.Combine(_env.WebRootPath, "uploads/books", existBook.BookImages.FirstOrDefault(x => x.IsPoster == false).Image);
                if (System.IO.File.Exists(deletePath))
                {
                    System.IO.File.Delete(deletePath);
                }
                existBook.BookImages.FirstOrDefault(x => x.IsPoster == false).Image = book.HoverImageFile.SaveFile(_env.WebRootPath, "uploads/books");
            }

            if (book.ImageFiles is not null)
            {
                foreach (IFormFile file in book.ImageFiles)
                {
                    if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
                    {
                        ModelState.AddModelError("ImageFiles", "You can only upload png or jpeg files");
                        return View();
                    }

                    if (file.Length > 2097152)
                    {
                        ModelState.AddModelError("ImageFiles", "You can only upload images under 2 mb");
                        return View();
                    }



                    BookImage bookImage = new BookImage
                    {
                        BookId = book.Id,
                        Image = file.SaveFile(_env.WebRootPath, "uploads/books"),
                        IsPoster = null
                    };
                    existBook.BookImages.Add(bookImage);

                }
            }


            existBook.Name = book.Name;
            existBook.CostPrice = book.CostPrice;
            existBook.SalePrice = book.SalePrice;
            existBook.DiscountPrice = book.DiscountPrice;
            existBook.CategoryId = book.CategoryId;
            existBook.AuthorId = book.AuthorId;
            existBook.Code = book.Code;
            existBook.Desc = book.Desc;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            Book book = _context.Books.Find(id);
            if (book == null) return NotFound();
            _context.Books.Remove(book);
            _context.SaveChanges();
            return Ok();
        }

        
    }
}
