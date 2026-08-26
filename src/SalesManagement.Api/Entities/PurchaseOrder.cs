using System;
using System.Collections.Generic;

namespace SalesManagement.Api.Entities
{
    public class PurchaseOrder : IMustHaveTenant
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public PurchaseStatus Status { get; set; } = PurchaseStatus.PENDING;
        public double Amount { get; set; }
        public string PoNumber { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
        public ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>();
    }
}
