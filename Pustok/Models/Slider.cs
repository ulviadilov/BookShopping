using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pustok.Models
{
    public class Slider
    {
        public int Id { get; set; }
        [StringLength(maximumLength:100)]
        public string? Image { get; set; }
        public string Title1 { get; set; }
        public string Title2 { get; set; }
        public string Desc { get; set; }
        public double Price { get; set; }
        public string RedirectUrl { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

    }
}
