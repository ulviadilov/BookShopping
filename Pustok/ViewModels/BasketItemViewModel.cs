using Pustok.Models;

namespace Pustok.ViewModels
{
    public class BasketItemViewModel
    {
        public int BookId { get; set; }
        public int Count { get; set; }
		public Book? Book { get; internal set; }
	}
}
