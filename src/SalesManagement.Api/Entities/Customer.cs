using System;
using System.Collections.Generic;

namespace SalesManagement.Api.Entities
{
    public class Customer : IMustHaveTenant
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public double LoyaltyPoint { get; set; } = 0.0;
        public double TotalSpent { get; set; } = 0.0;

        public Guid TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
    }
}
