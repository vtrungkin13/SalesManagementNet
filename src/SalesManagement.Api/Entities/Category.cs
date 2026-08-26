using System;
using System.Collections.Generic;

namespace SalesManagement.Api.Entities
{
    public class Category : IMustHaveTenant
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public Guid TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
