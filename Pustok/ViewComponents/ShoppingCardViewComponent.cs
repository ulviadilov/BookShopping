using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Newtonsoft.Json;
using Pustok.Models;
using Pustok.ViewModels;

namespace Pustok.ViewComponents
{
	public class ShoppingCardViewComponent : ViewComponent
	{
		private readonly PustokContext _context;

		public ShoppingCardViewComponent(PustokContext context)
		{
			_context = context;
		}

		public IViewComponentResult Invoke()
		{
			
			return View();
		}
	}
}
