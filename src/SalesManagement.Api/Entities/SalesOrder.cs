using System;
using System.Collections.Generic;

namespace SalesManagement.Api.Entities
{
    public class SalesOrder : IMustHaveTenant
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = null!;
        public double Subtotal { get; set; }
        public double Discount { get; set; }
        public double Total { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public Guid TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public ICollection<SalesOrderItem> SalesOrderItems { get; set; } = new List<SalesOrderItem>();
        public ICollection<ReturnOrder> ReturnOrders { get; set; } = new List<ReturnOrder>();
    }
}
