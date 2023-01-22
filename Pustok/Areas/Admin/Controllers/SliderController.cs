using Microsoft.AspNetCore.Mvc;
using Pustok.Helpers;
using Pustok.Models;

namespace Pustok.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController : Controller
    {
        private readonly PustokContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(PustokContext context , IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            
            return View(_context.Sliders.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Slider slider)
        {
            if (slider.ImageFile != null)
            {
                if (slider.ImageFile.ContentType != "image/jpeg" && slider.ImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "You can only upload png or jpeg files");
                    return View();
                }

                if (slider.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "You can only upload images under 2 mb");
                    return View();
                }
                slider.Image = slider.ImageFile.SaveFile(_env.WebRootPath, "uploads/sliders");
            }
            else
            {
                ModelState.AddModelError("ImageFile" ,"Required");
                return View();
            }

            if (!ModelState.IsValid) return View();
            _context.Sliders.Add(slider);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id) 
        {
            Slider slider = _context.Sliders.Find(id);
            if(slider == null) return View();
            return View(slider);
        }

        [HttpPost]
        public IActionResult Edit(Slider slider)
        {
            Slider existSlider = _context.Sliders.Find(slider.Id);
            if(existSlider is null) return View("Error");
            if (!ModelState.IsValid) return View();
            if(slider.ImageFile != null)
            {
                if (slider.ImageFile.ContentType != "image/png" && slider.ImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFile", "You can only upload png or jpeg file");
                    return View();
                }

                if(slider.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile" , "You can only upload files lower than 2mb");
                    return View();
                }
                string deletePath = Path.Combine(_env.WebRootPath, "uploads/sliders", existSlider.Image);
                existSlider.Image = slider.ImageFile.SaveFile(_env.WebRootPath, "uploads/sliders");
            }

            existSlider.Title1 = slider.Title1;
            existSlider.Title2 = slider.Title2;
            existSlider.RedirectUrl = slider.RedirectUrl;
            existSlider.Price = slider.Price;
            existSlider.Desc = slider.Desc;
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Slider slider = _context.Sliders.Find(id);
            if (slider == null) return NotFound();
            _context.Sliders.Remove(slider);
            _context.SaveChanges();
            return Ok();
        }
    }
}
