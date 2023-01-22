using Pustok.Enums;

namespace Pustok.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? AppUserId { get; set; }
        public bool IsDeleted { get; set; }
        public double TotalPrice { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Adress { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string ZipCode { get; set; }
        public string OrderNote { get; set; }
        public AppUser? AppUser { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime CreatedTime { get; set; }
        public List<OrderItem>? OrderItems { get; set; }

    }
}
