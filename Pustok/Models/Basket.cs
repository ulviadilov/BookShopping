namespace Pustok.Models
{
    public class Basket
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public List<BasketItem>? BasketItems { get; set; }
        public AppUser? User { get; set; }
    }
}
