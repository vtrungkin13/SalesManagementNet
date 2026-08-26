using System;

namespace SalesManagement.Api.Entities
{
    public class ProductImage
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = null!;

        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
