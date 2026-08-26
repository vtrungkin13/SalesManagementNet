using System;
using System.Collections.Generic;

namespace SalesManagement.Api.Entities
{
    public class Supplier : IMustHaveTenant
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
        public string? Address { get; set; }

        public Guid TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }
}
