﻿namespace Pustok.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int OrderId { get; set; }
        
        public bool IsDeleted { get; set; }
        public int Count { get; set; }
        public Book? Book { get; set; }
        public Order? Order { get; set; }
    }
}
