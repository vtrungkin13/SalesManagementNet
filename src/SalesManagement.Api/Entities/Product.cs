using System;
using System.Collections.Generic;

namespace SalesManagement.Api.Entities
{
    public class Product : IMustHaveTenant
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public ProductStatus Status { get; set; } = ProductStatus.ACTIVE;

        public ProductImage? Image { get; set; }

        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }

        public Guid TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }
}
