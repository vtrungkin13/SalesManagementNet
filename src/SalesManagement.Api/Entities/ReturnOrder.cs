using System;
using System.Collections.Generic;

namespace SalesManagement.Api.Entities
{
    public class ReturnOrder
    {
        public Guid Id { get; set; }
        public string ReturnNumber { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public double TotalRefund { get; set; }
        public DateTime ReturnDate { get; set; } = DateTime.UtcNow;

        public Guid SalesOrderId { get; set; }
        public SalesOrder? SalesOrder { get; set; }

        public ICollection<ReturnOrderItem> ReturnOrderItems { get; set; } = new List<ReturnOrderItem>();
    }
}
