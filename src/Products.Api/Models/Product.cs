using System;

namespace Products.Api.Models
{
    public class Product
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Category { get; set; }
    }

}