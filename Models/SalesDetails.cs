﻿namespace WebApplicationDotNET.Models
{
    public class SalesDetails
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
